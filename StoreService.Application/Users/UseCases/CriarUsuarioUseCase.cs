using StoreService.Application.Security;
using StoreService.Application.Users;
using StoreService.Application.Users.DTOs;
using StoreService.Domain.Entities;
using StoreService.Application.Users.Interfaces;

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
            if (string.IsNullOrWhiteSpace(req.Nome)) 
                throw new ArgumentException("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(req.Email)) 
                throw new ArgumentException("Email é obrigatório.");

            if (string.IsNullOrWhiteSpace(req.Senha) || req.Senha.Trim().Length < 6)
                throw new ArgumentException("A senha deve ter no mínimo 6 caracteres.");

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