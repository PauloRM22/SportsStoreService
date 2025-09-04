namespace StoreService.Application.Estoque.DTOs
{
    public class EntradaEstoqueResponse
    {
        public Guid MovimentacaoId { get; set; }
        public int MovimentacaoCodigo { get; set; }
        public Guid ProdutoId { get; set; }
        public int ProdutoCodigo { get; set; }
        public int Quantidade { get; set; }
        public string NumeroNotaFiscal { get; set; } = "";
        public int SaldoAtual { get; set; }
        public DateTime CriadoEm { get; set; }
    }
}