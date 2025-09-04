using StoreService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Domain.Entities
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public string ClienteDocumento { get; set; } = string.Empty;
        public Guid? VendedorId { get; set; }
        public string VendedorNome { get; set; } = string.Empty;
        public decimal ValorTotal { get; set; }
        public StatusPedido Status { get; set; } = StatusPedido.Rascunho;
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }

        public Usuario? Vendedor { get; set; }
        public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
    }
}
