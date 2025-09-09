using FluentAssertions;
using Moq;
using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Pedidos.Interfaces;
using StoreService.Application.Pedidos.UseCases;
using StoreService.Domain.Entities;

namespace StoreService.Tests.UseCases;

public class PedidosUseCasesTests
{
    [Fact]
    public async Task CriarPedidoComItens_Request_nulo_deve_lancar()
    {
        var repo = new Mock<IPedidoRepository>(MockBehavior.Strict);
        var useCase = new CriarPedidoComItensUseCase(repo.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() => useCase.ExecuteAsync(null!, Guid.NewGuid(), default));
    }

    [Fact]
    public async Task CriarPedidoComItens_Deve_chamar_repo_e_mapear()
    {
        var repo = new Mock<IPedidoRepository>();
        var useCase = new CriarPedidoComItensUseCase(repo.Object);

        var req = new CreatePedidoRequest
        {
            ClienteDocumento = "12345678901",
            VendedorNome = "Vendedor 1",
            Itens = new List<PedidoItemRequest>
            {
                new() { ProdutoId = Guid.NewGuid(), Quantidade = 2 }
            }
        };

        var dom = new Pedido
        {
            Id = Guid.NewGuid(),
            Codigo = 3,
            ClienteDocumento = req.ClienteDocumento,
            VendedorId = Guid.NewGuid(),
            VendedorNome = req.VendedorNome,
            ValorTotal = 499.80m,
            CriadoEm = DateTime.UtcNow,
            Itens = new List<PedidoItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProdutoId = req.Itens[0].ProdutoId,
                    Quantidade = req.Itens[0].Quantidade,
                    PrecoUnitario = 249.90m,
                    Subtotal = 499.80m,
                    Produto = new Produto { Codigo = 1 }
                }
            }
        };

        repo.Setup(r => r.CriarComItensAsync(It.IsAny<Pedido>(), It.IsAny<List<PedidoItem>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dom);

        var resp = await useCase.ExecuteAsync(req, Guid.NewGuid(), default);

        repo.Verify(r => r.CriarComItensAsync(It.IsAny<Pedido>(), It.IsAny<List<PedidoItem>>(), It.IsAny<CancellationToken>()), Times.Once);
        resp.Id.Should().Be(dom.Id);
        resp.Codigo.Should().Be(3);
        resp.ValorTotal.Should().Be(499.80m);
        resp.Itens.Should().HaveCount(1);
        resp.Itens[0].ProdutoCodigo.Should().Be(1);
        resp.Itens[0].Subtotal.Should().Be(499.80m);
    }
}