namespace StoreService.Domain.Entities
{
    public class PedidoItem
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public Guid PedidoId { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }

        public Pedido? Pedido { get; set; }
        public Produto? Produto { get; set; }
    }
}