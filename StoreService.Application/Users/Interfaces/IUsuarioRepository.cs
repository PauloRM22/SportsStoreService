using StoreService.Domain.Entities;

namespace StoreService.Application.Users.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<IList<Usuario>> GetAllAsync(CancellationToken ct = default);
        Task CreateAsync(Usuario usuario, CancellationToken ct = default);
        Task UpdateAsync(Usuario usuario, CancellationToken ct = default);
        Task DeleteAsync(Usuario usuario, CancellationToken ct = default);
    }
}