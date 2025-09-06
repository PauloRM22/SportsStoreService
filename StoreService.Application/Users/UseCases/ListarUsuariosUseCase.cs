using StoreService.Application.Users;
using StoreService.Application.Users.DTOs;
using StoreService.Application.Users.Interfaces;

namespace StoreService.Application.Users.UseCases
{
    public class ListarUsuariosUseCase
    {
        private readonly IUsuarioRepository _repo;
        public ListarUsuariosUseCase(IUsuarioRepository repo) => _repo = repo;

        public async Task<IList<UsuarioResponse>> ExecuteAsync(CancellationToken ct = default)
        {
            var list = await _repo.GetAllAsync(ct);
            return list.Select(x => x.ToResponse()).ToList();
        }
    }
}