using StoreService.Application.Usuarios.DTOs;
using StoreService.Application.Usuarios.Interfaces;

namespace StoreService.Application.Usuarios.UseCases
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