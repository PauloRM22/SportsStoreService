using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.Interfaces;

namespace StoreService.Application.Pedidos.UseCases
{
    public class ObterPedidoPorIdUseCase
    {
        private readonly IPedidoRepository _repo;
        public ObterPedidoPorIdUseCase(IPedidoRepository repo) => _repo = repo;

        public async Task<PedidoResponse> ExecuteAsync(Guid id, CancellationToken ct = default)
        {
            var p = await _repo.ObterComItensAsync(id, ct)
                ?? throw new ArgumentException("Pedido não encontrado.");

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