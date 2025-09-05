using StoreService.Application.Users;
using StoreService.Application.Users.DTOs;
using StoreService.Domain.Repositories;

namespace StoreService.Application.Users.UseCases
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