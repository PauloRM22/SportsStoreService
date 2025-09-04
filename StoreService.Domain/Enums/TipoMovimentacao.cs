using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Domain.Enums
{
    public enum TipoMovimentacao : byte
    {
        Entrada = 0,
        Saida = 1,
        Ajuste = 2
    }
}
