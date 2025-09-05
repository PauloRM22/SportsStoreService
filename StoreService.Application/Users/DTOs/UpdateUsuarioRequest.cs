using StoreService.Domain.Enums;

namespace StoreService.Application.Users.DTOs
{
    public class UpdateUsuarioRequest
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public TipoUsuario TipoUsuario { get; set; }
    }
}