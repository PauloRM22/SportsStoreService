using StoreService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Domain.Entities
{
    public class MovimentacaoEstoque
    {
        public Guid Id { get; set; }
        public int Codigo { get; set; }
        public Guid ProdutoId { get; set; }
        public TipoMovimentacao Tipo { get; set; }
        public int Quantidade { get; set; }
        public string? NumeroNotaFiscal { get; set; }
        public Guid? PedidoId { get; set; }
        public string? Observacao { get; set; }
        public DateTime CriadoEm { get; set; }

        public Produto? Produto { get; set; }
        public Pedido? Pedido { get; set; }
    }
}
