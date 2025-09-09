using FluentAssertions;
using Moq;
using StoreService.Application.Estoque.DTOs;
using StoreService.Application.Estoque.Interfaces;
using StoreService.Application.Estoque.UseCases;
using StoreService.Domain.Entities;

namespace StoreService.Tests.UseCases;

public class EstoqueUseCasesTests
{
    [Fact]
    public async Task RegistrarEntrada_Deve_chamar_repo_e_mapear()
    {
        var repo = new Mock<IEstoqueRepository>();
        var use = new RegistrarEntradaEstoqueUseCase(repo.Object);

        var req = new CreateEntradaEstoqueRequest
        {
            ProdutoId = Guid.NewGuid(),
            Quantidade = 3,
            NumeroNotaFiscal = "NF-1",
            Observacao = "Compra"
        };

        var mov = new MovimentacaoEstoque
        {
            Id = Guid.NewGuid(),
            Codigo = 10,
            ProdutoId = req.ProdutoId,
            Quantidade = req.Quantidade,
            NumeroNotaFiscal = req.NumeroNotaFiscal,
            CriadoEm = DateTime.UtcNow
        };

        repo.Setup(r => r.RegistrarEntradaAsync(req.ProdutoId, req.Quantidade, req.NumeroNotaFiscal, req.Observacao, It.IsAny<CancellationToken>()))
            .ReturnsAsync((mov, saldoAtual: 13, produtoCodigo: 123));

        var resp = await use.ExecuteAsync(req, default);

        repo.Verify(r => r.RegistrarEntradaAsync(req.ProdutoId, req.Quantidade, req.NumeroNotaFiscal, req.Observacao, It.IsAny<CancellationToken>()), Times.Once);
        resp.MovimentacaoId.Should().Be(mov.Id);
        resp.ProdutoCodigo.Should().Be(123);
        resp.SaldoAtual.Should().Be(13);
    }
}