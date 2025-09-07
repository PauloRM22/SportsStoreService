using StoreService.Domain.Enums;
using System.Text.Json.Serialization;

namespace StoreService.Application.Usuarios.DTOs
{
    public class CreateUsuarioRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty; // só entra aqui, nunca sai

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TipoUsuario TipoUsuario { get; set; }
    }
}