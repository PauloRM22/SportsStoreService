namespace StoreService.Application.Produtos.DTOs
{
    public class UpdateProdutoRequest
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public bool Ativo { get; set; } = true;
    }
}