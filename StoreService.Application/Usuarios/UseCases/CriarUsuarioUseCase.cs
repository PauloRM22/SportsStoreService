using StoreService.Application.Security;
using StoreService.Application.Usuarios.DTOs;
using StoreService.Application.Usuarios.Interfaces;
using StoreService.Domain.Entities;

namespace StoreService.Application.Usuarios.UseCases
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
            var email = req.Email.Trim().ToLowerInvariant();
            var senha = req.Senha.Trim();

            var existing = await _repo.GetByEmailAsync(email, ct);
            if (existing is not null)
                throw new ArgumentException("Já existe um usuário com este e-mail.");

            var entity = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = req.Nome.Trim(),
                Email = email,
                TipoUsuario = req.TipoUsuario,
                SenhaHash = _hasher.Hash(senha), // hash só depois de validar
                CriadoEm = DateTime.UtcNow
            };

            await _repo.CreateAsync(entity, ct);
            return entity.ToResponse();
        }
    }
}