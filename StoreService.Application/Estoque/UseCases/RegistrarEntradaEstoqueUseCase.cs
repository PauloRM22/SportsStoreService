using StoreService.Application.Estoque.DTOs;
using StoreService.Application.Estoque.Interfaces;

namespace StoreService.Application.Estoque.UseCases
{
    public class RegistrarEntradaEstoqueUseCase
    {
        private readonly IEstoqueRepository _repo;
        public RegistrarEntradaEstoqueUseCase(IEstoqueRepository repo) => _repo = repo;

        public async Task<EntradaEstoqueResponse> ExecuteAsync(CreateEntradaEstoqueRequest input, CancellationToken ct = default)
        {
            var (mov, saldoAtual, produtoCodigo) = await _repo.RegistrarEntradaAsync(
                input.ProdutoId, input.Quantidade, input.NumeroNotaFiscal, input.Observacao, ct);

            return new EntradaEstoqueResponse
            {
                MovimentacaoId = mov.Id,
                MovimentacaoCodigo = mov.Codigo,
                ProdutoId = mov.ProdutoId,
                ProdutoCodigo = produtoCodigo,
                Quantidade = mov.Quantidade,
                NumeroNotaFiscal = mov.NumeroNotaFiscal ?? "",
                SaldoAtual = saldoAtual,
                CriadoEm = mov.CriadoEm
            };
        }
    }
}