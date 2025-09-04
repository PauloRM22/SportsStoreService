using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Domain.Entities
{
    public class ProdutoSaldo
    {
        public Guid ProdutoId { get; set; }
        public int QuantidadeAtual { get; set; }
        public DateTime AtualizadoEm { get; set; }

        public Produto? Produto { get; set; }
    }
}
