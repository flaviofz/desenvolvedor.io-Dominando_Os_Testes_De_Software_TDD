using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NerdStore.Core.DomainObjects;
using NerdStore.Core.Messages;
using NerdStore.Vendas.Application.Events;
using NerdStore.Vendas.Domain;

namespace NerdStore.Vendas.Application.Commands
{
    public class PedidoCommandHandler : 
        IRequestHandler<AdicionarItemPedidoCommand, bool>
    {
        private readonly IMediator _mediator;
        private readonly IPedidoRepository _pedidoRepository;

        public PedidoCommandHandler(
            IPedidoRepository pedidoRepository, 
            IMediator mediator)
        {
            this._pedidoRepository = pedidoRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(
            AdicionarItemPedidoCommand message, 
            CancellationToken cancellationToken)
        {
            var pedidoItem = new PedidoItem(message.ProdutoId, message.Nome, message.Quantidade, message.ValorUnitario);
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(message.ClienteId);
            pedido.AdicionarItem(pedidoItem);

            _pedidoRepository.Adicionar(pedido);

            await _mediator.Publish(new PedidoItemAdicionadoEvent(
                pedido.ClienteId, 
                pedido.Id,
                message.ProdutoId, 
                message.Nome, 
                message.ValorUnitario, 
                message.Quantidade),
                cancellationToken);

            return true;
        }
    }
}