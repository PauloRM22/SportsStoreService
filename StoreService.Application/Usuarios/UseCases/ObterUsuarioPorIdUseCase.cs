using StoreService.Application.Usuarios.DTOs;
using StoreService.Application.Usuarios.Interfaces;

namespace StoreService.Application.Usuarios.UseCases
{
    public class ObterUsuarioPorIdUseCase
    {
        private readonly IUsuarioRepository _repo;
        public ObterUsuarioPorIdUseCase(IUsuarioRepository repo) => _repo = repo;

        public async Task<UsuarioResponse> ExecuteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct) ?? throw new ArgumentException("Usuário não encontrado.");
            return entity.ToResponse();
        }
    }
}