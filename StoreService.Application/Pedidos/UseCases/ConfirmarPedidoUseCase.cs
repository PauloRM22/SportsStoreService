using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.Interfaces;

namespace StoreService.Application.Pedidos.UseCases
{
    public class ConfirmarPedidoUseCase
    {
        private readonly IPedidoRepository _repo;
        public ConfirmarPedidoUseCase(IPedidoRepository repo) => _repo = repo;

        public async Task<PedidoResponse> ExecuteAsync(Guid pedidoId, CancellationToken ct = default)
        {
            var p = await _repo.ConfirmarAsync(pedidoId, ct);

            return new PedidoResponse
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
                    ProdutoCodigo = i.Produto?.Codigo ?? 0,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }
    }
}