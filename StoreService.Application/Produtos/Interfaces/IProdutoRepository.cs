using StoreService.Domain.Entities;

namespace StoreService.Application.Produtos.Interfaces
{
    public interface IProdutoRepository
    {
        Task<Produto> AddAsync(Produto entity, CancellationToken ct);
        Task<Produto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<Produto>> GetAllAsync(CancellationToken ct);
        Task UpdateAsync(Produto entity, CancellationToken ct);
        Task RemoveAsync(Guid id, CancellationToken ct);
        Task<bool> ExistsByNomeAsync(string nome, CancellationToken ct);
    }
}