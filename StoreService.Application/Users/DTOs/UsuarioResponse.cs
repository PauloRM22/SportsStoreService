using StoreService.Domain.Enums;

namespace StoreService.Application.Users.DTOs
{
    public class UsuarioResponse
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}
