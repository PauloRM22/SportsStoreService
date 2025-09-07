using StoreService.Application.Auth.DTOs;
using StoreService.Application.Security;
using StoreService.Application.Usuarios.Interfaces;

namespace StoreService.Application.Auth.UseCases
{
    public class LoginUseCase
    {
        private readonly IUsuarioRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly ITokenService _tokenService;

        public LoginUseCase(IUsuarioRepository repo, IPasswordHasher hasher, ITokenService tokenService)
        {
            _repo = repo;
            _hasher = hasher;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> ExecuteAsync(LoginRequest req, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Senha))
                throw new ArgumentException("E-mail e senha são obrigatórios.");

            var email = req.Email.Trim().ToLowerInvariant();
            var user = await _repo.GetByEmailAsync(email, ct);
            if (user is null) throw new UnauthorizedAccessException("Credenciais inválidas.");

            if (!_hasher.Verify(user.SenhaHash, req.Senha))
                throw new UnauthorizedAccessException("Credenciais inválidas.");

            var token = _tokenService.Generate(user, out var expiresAtUtc);

            return new LoginResponse
            {
                Token = token,
                ExpiresAtUtc = expiresAtUtc,
                UsuarioId = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                Perfil = user.TipoUsuario.ToString()
            };
        }
    }
}