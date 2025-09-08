using FluentValidation.TestHelper;
using StoreService.Application.Estoque.DTOs;
using StoreService.Application.Validations.Estoque;
using Xunit;

namespace StoreService.Tests.Validators;

public class EstoqueValidatorsTests
{
    private readonly CreateEntradaEstoqueRequestValidator _validator = new();

    private static CreateEntradaEstoqueRequest Valid() => new()
    {
        ProdutoId = Guid.NewGuid(),
        Quantidade = 10,
        NumeroNotaFiscal = "NF-123",
        Observacao = "Compra fornecedor X"
    };

    [Fact]
    public void ProdutoId_obrigatorio()
    {
        var m = Valid(); m.ProdutoId = Guid.Empty;
        _validator.TestValidate(m).ShouldHaveValidationErrorFor(x => x.ProdutoId);
    }

    [Fact]
    public void Quantidade_maior_que_zero()
    {
        var m = Valid(); m.Quantidade = 0;
        _validator.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Quantidade);
    }

    [Fact]
    public void NumeroNotaFiscal_obrigatorio_e_limite_50()
    {
        var m1 = Valid(); m1.NumeroNotaFiscal = "";
        _validator.TestValidate(m1).ShouldHaveValidationErrorFor(x => x.NumeroNotaFiscal);

        var m2 = Valid(); m2.NumeroNotaFiscal = new string('9', 51);
        _validator.TestValidate(m2).ShouldHaveValidationErrorFor(x => x.NumeroNotaFiscal);
    }

    [Fact]
    public void Observacao_limite_250_quando_preenchida()
    {
        var m = Valid(); m.Observacao = new string('o', 251);
        _validator.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Observacao);
    }

    [Fact]
    public void Valido_sem_erros()
    {
        _validator.TestValidate(Valid()).ShouldNotHaveAnyValidationErrors();
    }
}
