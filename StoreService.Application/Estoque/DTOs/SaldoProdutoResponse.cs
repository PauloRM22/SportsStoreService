namespace StoreService.Application.Estoque.DTOs
{
    public class SaldoProdutoResponse
    {
        public Guid ProdutoId { get; set; }
        public int ProdutoCodigo { get; set; }
        public int QuantidadeAtual { get; set; }
        public DateTime AtualizadoEm { get; set; }
    }
}