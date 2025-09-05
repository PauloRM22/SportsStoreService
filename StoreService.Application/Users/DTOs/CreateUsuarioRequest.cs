using StoreService.Domain.Enums;

namespace StoreService.Application.Users.DTOs
{
    public class CreateUsuarioRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty; // só entra aqui, nunca sai
        public TipoUsuario TipoUsuario { get; set; }
    }
}