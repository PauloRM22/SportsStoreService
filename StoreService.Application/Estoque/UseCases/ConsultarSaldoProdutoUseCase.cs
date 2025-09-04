using StoreService.Application.Estoque.DTOs;
using StoreService.Application.Estoque.Interfaces;

namespace StoreService.Application.Estoque.UseCases
{
    public class ConsultarSaldoProdutoUseCase
    {
        private readonly IEstoqueRepository _repo;
        public ConsultarSaldoProdutoUseCase(IEstoqueRepository repo) => _repo = repo;

        public async Task<SaldoProdutoResponse> ExecuteAsync(Guid produtoId, CancellationToken ct = default)
        {
            var (saldo, produtoCodigo, atualizadoEm) = await _repo.ObterSaldoAsync(produtoId, ct);

            return new SaldoProdutoResponse
            {
                ProdutoId = produtoId,
                ProdutoCodigo = produtoCodigo,
                QuantidadeAtual = saldo,
                AtualizadoEm = atualizadoEm
            };
        }
    }
}