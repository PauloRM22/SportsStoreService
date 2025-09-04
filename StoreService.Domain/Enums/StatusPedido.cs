using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Domain.Enums
{
    public enum StatusPedido : byte
    {
        Rascunho = 0,
        Confirmado = 1,
        Cancelado = 2
    }
}