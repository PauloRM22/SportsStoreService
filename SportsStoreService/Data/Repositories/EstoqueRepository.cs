using Microsoft.EntityFrameworkCore;
using StoreService.Application.Estoque.Interfaces;
using StoreService.Domain.Entities;
using StoreService.Domain.Enums;

namespace SportsStoreService.Data.repositories
{
    public class EstoqueRepository : IEstoqueRepository
    {
        private readonly AppDbContext _db;
        public EstoqueRepository(AppDbContext db) => _db = db;

        public async Task<(MovimentacaoEstoque mov, int saldoAtual, int produtoCodigo)> RegistrarEntradaAsync(
            Guid produtoId, int quantidade, string numeroNotaFiscal, string? observacao, CancellationToken ct)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);

            var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == produtoId, ct)
                ?? throw new ArgumentException("Produto não encontrado.");

            var saldo = await _db.ProdutosSaldo.FirstOrDefaultAsync(s => s.ProdutoId == produtoId, ct);
            if (saldo is null)
            {
                saldo = new ProdutoSaldo { ProdutoId = produtoId, QuantidadeAtual = 0, AtualizadoEm = DateTime.UtcNow };
                _db.ProdutosSaldo.Add(saldo);
            }

            var mov = new MovimentacaoEstoque
            {
                Id = Guid.NewGuid(),
                ProdutoId = produtoId,
                Tipo = TipoMovimentacao.Entrada,
                Quantidade = quantidade,
                NumeroNotaFiscal = numeroNotaFiscal,
                Observacao = observacao,
                CriadoEm = DateTime.UtcNow
            };

            _db.MovimentacoesEstoque.Add(mov);

            saldo.QuantidadeAtual += quantidade;
            saldo.AtualizadoEm = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return (mov, saldo.QuantidadeAtual, produto.Codigo);
        }

        public async Task<(int saldo, int produtoCodigo, DateTime atualizadoEm)> ObterSaldoAsync(Guid produtoId, CancellationToken ct)
        {
            var produto = await _db.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == produtoId, ct)
                ?? throw new ArgumentException("Produto não encontrado.");

            var saldo = await _db.ProdutosSaldo.AsNoTracking().FirstOrDefaultAsync(s => s.ProdutoId == produtoId, ct);
            if (saldo is null)
                return (0, produto.Codigo, DateTime.MinValue);

            return (saldo.QuantidadeAtual, produto.Codigo, saldo.AtualizadoEm);
        }
    }
}