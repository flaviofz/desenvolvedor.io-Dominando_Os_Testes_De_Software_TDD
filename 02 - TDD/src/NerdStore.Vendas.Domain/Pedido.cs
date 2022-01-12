using NerdStore.Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public static int MAX_UNIDADES_ITEM => 15;
        public static int MIN_UNIDADES_ITEM => 1;

        public Guid ClienteId { get; private set; }
        public decimal ValorTotal { get; private set; }
        public PedidoStatus Status { get; private set; }

        private readonly List<PedidoItem> _pedidoItens;

        protected Pedido()
        {
            _pedidoItens = new List<PedidoItem>();
        }

        public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens;

        public void CalcularValorPedido()
        {
            ValorTotal = PedidoItens.Sum(x => x.CalcularValor());
        }

        public void TornarRascunho() => Status = PedidoStatus.Rascunho;

        private bool PedidoItemExistente(PedidoItem pedidoItem) => _pedidoItens.Any(p => p.ProdutoId == pedidoItem.ProdutoId);

        private void ValidarQuantidadeItemPermitida(PedidoItem pedidoItem)
        {
            var quantidadeItens = pedidoItem.Quantidade;
            if (PedidoItemExistente(pedidoItem))
            {
                var itemExistente = _pedidoItens.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
                quantidadeItens += itemExistente.Quantidade;
            }

            if (quantidadeItens > MAX_UNIDADES_ITEM) throw new DomainException($"Máximo de {MAX_UNIDADES_ITEM} unidades por produto.");
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            ValidarQuantidadeItemPermitida(pedidoItem);

            if (PedidoItemExistente(pedidoItem))
            {                
                var itemExistente = _pedidoItens.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
               
                itemExistente.AdicionarUnidades(pedidoItem.Quantidade);
                pedidoItem = itemExistente;
                _pedidoItens.Remove(itemExistente);
            }

            _pedidoItens.Add(pedidoItem);
            CalcularValorPedido();
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido
                {
                    ClienteId = clienteId,
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }
}
