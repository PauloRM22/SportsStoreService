namespace StoreService.Application.Pedidos.DTOs
{
    public class CreatePedidoRequest
    {
        public string ClienteDocumento { get; set; } = string.Empty;
        public string VendedorNome { get; set; } = string.Empty; // simples p/ teste
        public List<PedidoItemRequest> Itens { get; set; } = new();
    }

    public class PedidoItemRequest
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }
}