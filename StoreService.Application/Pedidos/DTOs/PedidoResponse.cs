using StoreService.Domain.Enums;

namespace StoreService.Application.Pedidos.DTOs
{
    public class PedidoResponse
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public string ClienteDocumento { get; set; } = "";
        public string VendedorNome { get; set; } = "";
        public decimal ValorTotal { get; set; }
        public StatusPedido Status { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public List<PedidoItemResponse> Itens { get; set; } = new();
    }

    public class PedidoItemResponse
    {
        public Guid ProdutoId { get; set; }
        public int ProdutoCodigo { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}