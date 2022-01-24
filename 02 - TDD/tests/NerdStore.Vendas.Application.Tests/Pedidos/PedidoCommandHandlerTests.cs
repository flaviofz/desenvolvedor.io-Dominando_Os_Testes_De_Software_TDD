using MediatR;
using Moq;
using Moq.AutoMock;
using NerdStore.Vendas.Application.Commands;
using NerdStore.Vendas.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NerdStore.Vendas.Application.Tests.Pedidos
{
    public class PedidoCommandHandlerTests
    {
        private readonly Guid _clienteId;
        private readonly Guid _produtoId;
        private readonly Pedido _pedido;
        private readonly AutoMocker _mocker;
        private readonly PedidoCommandHandler _pedidoCommandHandler;

        // O construtor da classe é chamado em cada teste rodado
        // Se for necessário manter o estado de um objeto deve ser usado Fixture
        public PedidoCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _pedidoCommandHandler = _mocker.CreateInstance<PedidoCommandHandler>();

            _clienteId = Guid.NewGuid();
            _produtoId = Guid.NewGuid();
            _pedido = Pedido.PedidoFactory.NovoPedidoRascunho(_clienteId);
        }

        [Fact(DisplayName = "Adicionar item novo pedido com sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async Task AdicionarItem_NovoPedido_DeveExecutarComSucesso()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(
                clienteId: _clienteId,
                produtoId: _produtoId,
                nome: "Produto Teste",
                quantidade: 2,
                valorUnitario: 100);

            _mocker.GetMock<IPedidoRepository>()
                .Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _pedidoCommandHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.Adicionar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
            //mocker.GetMock<IMediator>().Verify(x => x.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Adicionar novo item pedido rascunho com sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async void AdicionarItem_NovoItemAoPedidoRascunho_DeveExecutarComSucesso()
        {
            // Arrange            
            var pedidoItemExistente = new PedidoItem(_produtoId, "Produto Xpto", 2, 100);
            _pedido.AdicionarItem(pedidoItemExistente);

            var pedidoCommand = new AdicionarItemPedidoCommand(_clienteId, Guid.NewGuid(), "Produto Teste", 2, 100);

            _mocker.GetMock<IPedidoRepository>()
                .Setup(x => x.ObterPedidoRascunhoPorClienteId(_clienteId)).Returns(Task.FromResult(_pedido));

            _mocker.GetMock<IPedidoRepository>()
                .Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _pedidoCommandHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.AdicionarItem(It.IsAny<PedidoItem>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Adicionar item existente ao pedido rascunho com sucesso")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async void AdicionarItem_ItemExistenteAoPEdidoRascunho_DeveExecutarComSucesso()
        {
            // Arrange
            var pedidoItemExistente = new PedidoItem(_produtoId, "Produto Xpto", 2, 100);
            _pedido.AdicionarItem(pedidoItemExistente);

            var pedidoCommand = new AdicionarItemPedidoCommand(_clienteId, _produtoId, "Produto Xpto", 2, 100);

            _mocker.GetMock<IPedidoRepository>()
                .Setup(x => x.ObterPedidoRascunhoPorClienteId(_clienteId)).Returns(Task.FromResult(_pedido));

            _mocker.GetMock<IPedidoRepository>()
                .Setup(x => x.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _pedidoCommandHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.AtualizarItem(It.IsAny<PedidoItem>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.Atualizar(It.IsAny<Pedido>()), Times.Once);
            _mocker.GetMock<IPedidoRepository>().Verify(x => x.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Adicionar item command inválido")]
        [Trait("Categoria", "Vendas - Pedido Command Handler")]
        public async void AdicionarItem_CommandInvalido_DeveRetornarFalsoELancarEventosDeNotificacao()
        {
            // Arrange
            var pedidoCommand = new AdicionarItemPedidoCommand(
                Guid.Empty,
                Guid.Empty,
                "",
                0,
                0);

            // Act
            var result = await _pedidoCommandHandler.Handle(pedidoCommand, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mocker.GetMock<IMediator>().Verify(x => x.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Exactly(5));
        }
    }
}
