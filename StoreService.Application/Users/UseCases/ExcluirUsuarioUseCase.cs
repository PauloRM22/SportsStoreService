using StoreService.Application.Users.Interfaces;

namespace StoreService.Application.Users.UseCases
{
    public class ExcluirUsuarioUseCase
    {
        private readonly IUsuarioRepository _repo;
        public ExcluirUsuarioUseCase(IUsuarioRepository repo) => _repo = repo;

        public async Task ExecuteAsync(Guid id, CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct) ?? throw new ArgumentException("Usuário não encontrado.");
            await _repo.DeleteAsync(entity, ct);
        }
    }
}