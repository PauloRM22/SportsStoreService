using StoreService.Domain.Entities;

namespace StoreService.Application.Pedidos.Interfaces
{
    public interface IPedidoRepository
    {
        Task<Pedido> CriarComItensAsync(Pedido pedido, List<PedidoItem> itens, CancellationToken ct);
        Task<Pedido?> ObterComItensAsync(Guid pedidoId, CancellationToken ct);
        Task<Pedido> ConfirmarAsync(Guid pedidoId, CancellationToken ct);
    }
}