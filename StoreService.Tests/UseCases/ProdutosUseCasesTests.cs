using FluentAssertions;
using Moq;
using StoreService.Application.Produtos.DTOs;
using StoreService.Application.Produtos.Interfaces;
using StoreService.Application.Produtos.UseCases;
using StoreService.Domain.Entities;

namespace StoreService.Tests.UseCases;

public class ProdutosUseCasesTests
{
    [Fact]
    public async Task CriarProduto_Input_nulo_deve_lancar()
    {
        var repo = new Mock<IProdutoRepository>(MockBehavior.Strict);
        var use = new CriarProdutoUseCase(repo.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(() => use.ExecuteAsync(null!, default));
    }

    [Fact]
    public async Task CriarProduto_Deve_chamar_repo_e_retornar_mapeado()
    {
        var repo = new Mock<IProdutoRepository>();
        var use = new CriarProdutoUseCase(repo.Object);

        var input = new CreateProdutoRequest { Nome = "Bola", Descricao = "Oficial", Preco = 10m };
        var entity = new Produto
        {
            Id = Guid.NewGuid(),
            Codigo = 5,
            Nome = "Bola",
            Descricao = "Oficial",
            Preco = 10m,
            Ativo = true,
            CriadoEm = DateTime.UtcNow
        };

        repo.Setup(r => r.AddAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var resp = await use.ExecuteAsync(input, default);

        repo.Verify(r => r.AddAsync(It.IsAny<Produto>(), It.IsAny<CancellationToken>()), Times.Once);
        resp.Id.Should().Be(entity.Id);
        resp.Codigo.Should().Be(5);
        resp.Nome.Should().Be("Bola");
        resp.Preco.Should().Be(10m);
    }
}