using StoreService.Application.Products.Interfaces;

namespace StoreService.Application.Products.UseCases
{
    public class RemoverProdutoUseCase
    {
        private readonly IProdutoRepository _repo;
        public RemoverProdutoUseCase(IProdutoRepository repo) => _repo = repo;

        public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
        {
            var p = await _repo.GetByIdAsync(id, ct)
                ?? throw new ArgumentException("Produto não encontrado.");

            await _repo.RemoveAsync(id, ct);
        }
    }
}