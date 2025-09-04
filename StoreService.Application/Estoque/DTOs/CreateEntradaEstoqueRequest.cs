namespace StoreService.Application.Estoque.DTOs
{
    public class CreateEntradaEstoqueRequest
    {
        public Guid ProdutoId { get; set; }
        public int Quantidade { get; set; }
        public string NumeroNotaFiscal { get; set; } = "";
        public string? Observacao { get; set; }
    }
}