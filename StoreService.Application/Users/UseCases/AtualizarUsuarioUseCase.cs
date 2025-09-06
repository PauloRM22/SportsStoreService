using StoreService.Application.Users;
using StoreService.Application.Users.DTOs;
using StoreService.Application.Users.Interfaces;

namespace StoreService.Application.Users.UseCases
{
    public class AtualizarUsuarioUseCase
    {
        private readonly IUsuarioRepository _repo;
        public AtualizarUsuarioUseCase(IUsuarioRepository repo) => _repo = repo;

        public async Task<UsuarioResponse> ExecuteAsync(UpdateUsuarioRequest req, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(req.Id, ct) ?? throw new ArgumentException("Usuário não encontrado.");

            entity.Nome = req.Nome.Trim();
            entity.Email = req.Email.Trim().ToLowerInvariant();
            entity.TipoUsuario = req.TipoUsuario;
            entity.AtualizadoEm = DateTime.UtcNow;

            await _repo.UpdateAsync(entity, ct);
            return entity.ToResponse();
        }
    }
}