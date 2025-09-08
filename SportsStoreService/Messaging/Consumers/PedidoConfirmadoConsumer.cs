using MassTransit;
using Microsoft.Extensions.Logging;
using StoreService.Application.Messaging.Events;

namespace SportsStoreService.Messaging.Consumers
{
    public class PedidoConfirmadoConsumer : IConsumer<PedidoConfirmadoEvent>
    {
        private readonly ILogger<PedidoConfirmadoConsumer> _log;
        public PedidoConfirmadoConsumer(ILogger<PedidoConfirmadoConsumer> log) => _log = log;

        public Task Consume(ConsumeContext<PedidoConfirmadoEvent> ctx)
        {
            var m = ctx.Message;
            _log.LogInformation(
                "Pedido confirmado #{Codigo} (R${Valor}) para {Cliente} por {Vendedor}. Itens: {Qtd}",
                m.PedidoCodigo, m.ValorTotal, m.ClienteDocumento, m.VendedorNome, m.Itens.Count);
            return Task.CompletedTask;
        }
    }
}