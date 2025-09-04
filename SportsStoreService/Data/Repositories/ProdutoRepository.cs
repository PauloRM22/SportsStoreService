using Microsoft.EntityFrameworkCore;
using SportsStoreService.Data;
using StoreService.Application.Products.Interfaces;
using StoreService.Domain.Entities;

namespace SportsStoreService.Data.Repositories
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly AppDbContext _db;
        public ProdutoRepository(AppDbContext db) => _db = db;

        public async Task<Produto> AddAsync(Produto entity, CancellationToken ct)
        {
            _db.Produtos.Add(entity);
            await _db.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<List<Produto>> GetAllAsync(CancellationToken ct)
            => await _db.Produtos.AsNoTracking().ToListAsync(ct);

        public async Task<Produto?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task UpdateAsync(Produto entity, CancellationToken ct)
        {
            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(Guid id, CancellationToken ct)
        {
            var entity = await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return;
            _db.Produtos.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsByNomeAsync(string nome, Guid? exceptId, CancellationToken ct)
        {
            var q = _db.Produtos.AsQueryable();
            if (exceptId.HasValue)
                return await q.AnyAsync(p => p.Nome == nome && p.Id != exceptId.Value, ct);
            return await q.AnyAsync(p => p.Nome == nome, ct);
        }
    }
}
