namespace StoreService.Application.Messaging.Events;

public record PedidoConfirmadoEvent(
    Guid PedidoId,
    int PedidoCodigo,
    Guid VendedorId,
    string VendedorNome,
    string ClienteDocumento,
    decimal ValorTotal,
    DateTime ConfirmadoEmUtc,
    IReadOnlyList<PedidoItemDto> Itens);

public record PedidoItemDto(Guid ProdutoId, int ProdutoCodigo, int Quantidade, decimal PrecoUnitario, decimal Subtotal);