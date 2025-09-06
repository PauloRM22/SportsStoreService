using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.Interfaces;
using StoreService.Domain.Entities;
using StoreService.Domain.Enums;

namespace StoreService.Application.Pedidos.UseCases
{
    public class CriarPedidoComItensUseCase
    {
        private readonly IPedidoRepository _repo;
        public CriarPedidoComItensUseCase(IPedidoRepository repo) => _repo = repo;

        public async Task<PedidoResponse> ExecuteAsync(CreatePedidoRequest input, CancellationToken ct = default)
        { 
            var pedido = new Pedido
            {
                Id = Guid.NewGuid(),
                ClienteDocumento = input.ClienteDocumento.Trim(),
                VendedorNome = input.VendedorNome.Trim(),
                ValorTotal = 0,
                Status = StatusPedido.Rascunho,
                CriadoEm = DateTime.UtcNow
            };

            var itens = input.Itens
                .GroupBy(i => i.ProdutoId)
                .Select(g => new PedidoItem
                {
                    Id = Guid.NewGuid(),
                    PedidoId = pedido.Id,
                    ProdutoId = g.Key,
                    Quantidade = g.Sum(x => x.Quantidade), // consolida duplicados
                    PrecoUnitario = 0 // será preenchido no repo a partir do Produto.Preco
                })
                .ToList();

            pedido = await _repo.CriarComItensAsync(pedido, itens, ct);
            return Map(pedido);
        }

        private static PedidoResponse Map(Pedido p) => new()
        {
            Id = p.Id,
            Codigo = p.Codigo,
            ClienteDocumento = p.ClienteDocumento,
            VendedorNome = p.VendedorNome,
            ValorTotal = p.ValorTotal,
            Status = p.Status,
            CriadoEm = p.CriadoEm,
            AtualizadoEm = p.AtualizadoEm,
            Itens = p.Itens.Select(i => new PedidoItemResponse
            {
                ProdutoId = i.ProdutoId,
                ProdutoCodigo = 0, // preenchido no GET por id (repo vai incluir Produto)
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}