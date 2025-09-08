using StoreService.Application.Messaging;
using StoreService.Application.Messaging.Events;
using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.Interfaces;

namespace StoreService.Application.Pedidos.UseCases
{
    public class ConfirmarPedidoUseCase
    {
        private readonly IPedidoRepository _repo;
        private readonly IEventPublisher _bus;
        public ConfirmarPedidoUseCase(IPedidoRepository repo, IEventPublisher bus)
        {
            _repo = repo;
            _bus = bus;
        }

        public async Task<PedidoResponse> ExecuteAsync(Guid pedidoId, CancellationToken ct = default)
        {
            var p = await _repo.ConfirmarAsync(pedidoId, ct);

            if (!p.VendedorId.HasValue)
                throw new ArgumentException("Pedido sem VendedorId não pode ser confirmado.");

            var evt = new PedidoConfirmadoEvent(
                p.Id,
                p.Codigo,
                p.VendedorId.Value,
                p.VendedorNome,
                p.ClienteDocumento,
                p.ValorTotal,
                DateTime.UtcNow,
                p.Itens.Select(i => new PedidoItemDto(
                    i.ProdutoId,
                    i.Produto?.Codigo ?? 0,
                    i.Quantidade,
                    i.PrecoUnitario,
                    i.Subtotal)).ToList()
            );

            await _bus.PublishAsync(evt, ct);

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