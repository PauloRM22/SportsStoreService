using StoreService.Application.Security;
using StoreService.Application.Users;
using StoreService.Application.Users.DTOs;
using StoreService.Domain.Entities;
using StoreService.Domain.Repositories;

namespace StoreService.Application.Users.UseCases
{
    public class CriarUsuarioUseCase
    {
        private readonly IUsuarioRepository _repo;
        private readonly IPasswordHasher _hasher;

        public CriarUsuarioUseCase(IUsuarioRepository repo, IPasswordHasher hasher)
        {
            _repo = repo;
            _hasher = hasher;
        }

        public async Task<UsuarioResponse> ExecuteAsync(CreateUsuarioRequest req, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(req.Nome)) throw new ArgumentException("Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(req.Email)) throw new ArgumentException("Email é obrigatório.");
            if (string.IsNullOrWhiteSpace(req.Senha)) throw new ArgumentException("Senha é obrigatória.");

            var existing = await _repo.GetByEmailAsync(req.Email.Trim().ToLowerInvariant(), ct);
            if (existing is not null) throw new ArgumentException("Já existe um usuário com este e-mail.");

            var entity = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = req.Nome.Trim(),
                Email = req.Email.Trim().ToLowerInvariant(),
                TipoUsuario = req.TipoUsuario,
                SenhaHash = _hasher.Hash(req.Senha),
                CriadoEm = DateTime.UtcNow
            };

            await _repo.CreateAsync(entity, ct);
            return entity.ToResponse();
        }
    }
}