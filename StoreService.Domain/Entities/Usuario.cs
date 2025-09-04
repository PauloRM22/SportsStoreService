using StoreService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // único
        public byte[] SenhaHash { get; set; } = Array.Empty<byte>();
        public TipoUsuario TipoUsuario { get; set; }
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
    }
}