using System;
using System.Collections.Generic;
using System.Text;

namespace NerdStore.Vendas.Domain
{
    public interface IPedidoRepository
    {
        void Adicionar(Pedido pedido);
    }
}
