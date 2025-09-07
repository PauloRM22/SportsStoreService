using Microsoft.EntityFrameworkCore;
using StoreService.Application.Pedidos.Interfaces;
using StoreService.Domain.Entities;
using StoreService.Domain.Enums;

namespace StoreService.Infrastructure.Data.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly AppDbContext _db;
        public PedidoRepository(AppDbContext db) => _db = db;

        public async Task<Pedido> CriarComItensAsync(Pedido pedido, List<PedidoItem> itens, CancellationToken ct)
        {
            // Carrega preços atuais e valida produtos
            var produtoIds = itens.Select(i => i.ProdutoId).Distinct().ToList();
            var produtos = await _db.Produtos.Where(p => produtoIds.Contains(p.Id)).ToListAsync(ct);
            if (produtos.Count != produtoIds.Count)
                throw new ArgumentException("Algum produto informado não foi encontrado.");

            var precoPorProduto = produtos.ToDictionary(p => p.Id, p => p.Preco);

            foreach (var item in itens)
            {
                item.PrecoUnitario = precoPorProduto[item.ProdutoId];
            }

            pedido.Itens = itens;
            pedido.ValorTotal = itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync(ct);
            return await ObterComItensAsync(pedido.Id, ct) ?? pedido;
        }

        public async Task<Pedido?> ObterComItensAsync(Guid pedidoId, CancellationToken ct)
        {
            return await _db.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == pedidoId, ct);
        }

        public async Task<Pedido> ConfirmarAsync(Guid pedidoId, CancellationToken ct)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var pedido = await _db.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == pedidoId, ct)
                ?? throw new ArgumentException("Pedido não encontrado.");

            if (pedido.Status != StatusPedido.Rascunho)
                throw new ArgumentException("Somente pedidos em rascunho podem ser confirmados.");

            // Valida saldos antes
            var produtoIds = pedido.Itens.Select(i => i.ProdutoId).Distinct().ToList();
            var saldos = await _db.ProdutosSaldo
                .Where(s => produtoIds.Contains(s.ProdutoId))
                .ToDictionaryAsync(s => s.ProdutoId, s => s, ct);

            foreach (var item in pedido.Itens)
            {
                saldos.TryGetValue(item.ProdutoId, out var saldo);
                var disponivel = saldo?.QuantidadeAtual ?? 0;
                if (disponivel < item.Quantidade)
                {
                    // recupera codigo do produto pra msg amigável
                    var prod = await _db.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == item.ProdutoId, ct);
                    var codigo = prod?.Codigo ?? 0;
                    throw new ArgumentException($"Estoque insuficiente para o produto {codigo}.");
                }
            }

            // Debita estoque e cria movimentações
            foreach (var item in pedido.Itens)
            {
                var saldo = await _db.ProdutosSaldo.FirstOrDefaultAsync(s => s.ProdutoId == item.ProdutoId, ct);
                if (saldo == null)
                {
                    saldo = new ProdutoSaldo { ProdutoId = item.ProdutoId, QuantidadeAtual = 0, AtualizadoEm = DateTime.UtcNow };
                    _db.ProdutosSaldo.Add(saldo);
                }

                saldo.QuantidadeAtual -= item.Quantidade;
                saldo.AtualizadoEm = DateTime.UtcNow;

                _db.MovimentacoesEstoque.Add(new MovimentacaoEstoque
                {
                    Id = Guid.NewGuid(),
                    ProdutoId = item.ProdutoId,
                    Tipo = TipoMovimentacao.Saida,
                    Quantidade = item.Quantidade,
                    NumeroNotaFiscal = null,
                    PedidoId = pedido.Id,
                    Observacao = "Baixa por confirmação de pedido",
                    CriadoEm = DateTime.UtcNow
                });
            }

            // Atualiza pedido
            pedido.ValorTotal = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
            pedido.Status = StatusPedido.Confirmado;
            pedido.AtualizadoEm = DateTime.UtcNow;

            // IMPORTANTE: não usar Update(pedido) para não sujar Codigo (IDENTITY)
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            // Retorna com Produto incluido
            return await ObterComItensAsync(pedido.Id, ct) ?? pedido;
        }
    }
}