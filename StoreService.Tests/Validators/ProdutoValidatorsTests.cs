using FluentValidation.TestHelper;
using StoreService.Application.Produtos.DTOs;
using StoreService.Application.Validations.Produtos;

namespace StoreService.Tests.Validators;

public class ProdutoValidatorsTests
{
    private readonly CreateProdutoRequestValidator _create = new();
    private readonly UpdateProdutoRequestValidator _update = new();

    private static CreateProdutoRequest ValidCreate() => new()
    {
        Nome = "Bola de Futebol",
        Descricao = "Bola oficial",
        Preco = 199.90m
    };

    private static UpdateProdutoRequest ValidUpdate() => new()
    {
        Id = Guid.NewGuid(),
        Nome = "Bola de Futebol",
        Descricao = "Bola oficial",
        Preco = 199.90m
    };

    [Fact]
    public void Create_Nome_obrigatorio()
    {
        var m = ValidCreate(); m.Nome = "";
        _create.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Create_Preco_maior_que_zero()
    {
        var m = ValidCreate(); m.Preco = 0m;
        _create.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Preco);
    }

    [Fact]
    public void Create_Descricao_limite_1000()
    {
        var m = ValidCreate(); m.Descricao = new string('x', 1001);
        _create.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Descricao);
    }

    [Fact]
    public void Create_valido_sem_erros()
    {
        _create.TestValidate(ValidCreate()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Update_Id_obrigatorio()
    {
        var m = ValidUpdate(); m.Id = Guid.Empty;
        _update.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Update_Nome_obrigatorio()
    {
        var m = ValidUpdate(); m.Nome = "  ";
        _update.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Update_Preco_maior_que_zero()
    {
        var m = ValidUpdate(); m.Preco = 0m;
        _update.TestValidate(m).ShouldHaveValidationErrorFor(x => x.Preco);
    }

    [Fact]
    public void Update_valido_sem_erros()
    {
        _update.TestValidate(ValidUpdate()).ShouldNotHaveAnyValidationErrors();
    }
}