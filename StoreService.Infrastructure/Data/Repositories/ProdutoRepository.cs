using Microsoft.EntityFrameworkCore;
using StoreService.Application.Produtos.Interfaces;
using StoreService.Domain.Entities;

namespace StoreService.Infrastructure.Data.Repositories
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
            => await _db.Produtos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

        public async Task UpdateAsync(Produto entity, CancellationToken ct)
        {
            // Anexa e marca apenas os campos que podem mudar.
            _db.Attach(entity);

            var entry = _db.Entry(entity);
            entry.Property(p => p.Nome).IsModified = true;
            entry.Property(p => p.Descricao).IsModified = true;
            entry.Property(p => p.Preco).IsModified = true;
            entry.Property(p => p.Ativo).IsModified = true;
            entry.Property(p => p.AtualizadoEm).IsModified = true;

            // NUNCA mexer em Identity/Keys:
            // entry.Property(p => p.Codigo).IsModified = false; // (desnecessário, só pra reforçar)

            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(Guid id, CancellationToken ct)
        {
            var entity = await _db.Produtos.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return;

            _db.Produtos.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Verifica se já existe um produto com o mesmo nome.
        /// </summary>
        public async Task<bool> ExistsByNomeAsync(string nome, CancellationToken ct)
        {
            return await _db.Produtos.AnyAsync(p => p.Nome == nome, ct);
        }
    }
}