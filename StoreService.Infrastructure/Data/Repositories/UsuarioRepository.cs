using Microsoft.EntityFrameworkCore;
using StoreService.Application.Usuarios.Interfaces;
using StoreService.Domain.Entities;

namespace StoreService.Infrastructure.Data.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _ctx;
        public UsuarioRepository(AppDbContext ctx) => _ctx = ctx;

        public Task<Usuario?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct = default) =>
            _ctx.Usuarios.AsNoTracking().FirstOrDefaultAsync(x => x.Email == email, ct);

        public async Task<IList<Usuario>> GetAllAsync(CancellationToken ct = default) =>
            await _ctx.Usuarios.AsNoTracking().OrderBy(x => x.Codigo).ToListAsync(ct);

        public async Task CreateAsync(Usuario usuario, CancellationToken ct = default)
        {
            await _ctx.Usuarios.AddAsync(usuario, ct);
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Usuario usuario, CancellationToken ct = default)
        {
            await _ctx.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Usuario usuario, CancellationToken ct = default)
        {
            _ctx.Usuarios.Remove(usuario);
            await _ctx.SaveChangesAsync(ct);
        }
    }
}