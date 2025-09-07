namespace StoreService.Application.Produtos.DTOs
{
    public class CreateProdutoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
    }
}