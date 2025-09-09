using FluentValidation.TestHelper;
using StoreService.Application.Usuarios.DTOs;
using StoreService.Application.Validations.Usuarios;
using StoreService.Domain.Enums;

namespace StoreService.Tests.Validators;

public class UsuariosValidatorsTests
{
    private readonly CreateUsuarioRequestValidator _validator = new();

    private static CreateUsuarioRequest Valid() => new()
    {
        Nome = "João da Silva",
        Email = "joao.silva@dominio.com",
        Senha = "segredo123",
        TipoUsuario = TipoUsuario.Administrador
    };

    [Fact]
    public void Nome_nao_pode_ser_vazio()
    {
        var model = Valid();
        model.Nome = "";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorMessage("Nome é obrigatório");
    }

    [Fact]
    public void Nome_nao_pode_ser_branco()
    {
        var model = Valid();
        model.Nome = "   ";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorMessage("Nome é obrigatório");
    }

    [Fact]
    public void Nome_deve_ter_no_maximo_100_caracteres()
    {
        var model = Valid();
        model.Nome = new string('a', 101);
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Nome)
              .WithErrorMessage("Nome deve ter no máximo 100 caracteres.");
    }

    [Fact]
    public void Nome_valido_nao_deve_gerar_erro()
    {
        var model = Valid();
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Nome);
    }

    [Fact]
    public void Email_nao_pode_ser_vazio()
    {
        var model = Valid();
        model.Email = "";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("E-mail é obrigatório");
    }

    [Theory]
    [InlineData("invalido")]
    [InlineData("a@")]
    [InlineData("@b.com")]
    public void Email_invalido_deve_falhar(string email)
    {
        var model = Valid();
        model.Email = email;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("E-mail inválido.");
    }


    [Fact]
    public void Email_valido_nao_deve_gerar_erro()
    {
        var model = Valid();
        model.Email = "ok@dominio.com";
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Senha_nao_pode_ser_vazia()
    {
        var model = Valid();
        model.Senha = "";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Senha)
              .WithErrorMessage("Senha é obrigatória");
    }

    [Fact]
    public void Senha_nao_pode_ser_branco()
    {
        var model = Valid();
        model.Senha = "   ";
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Senha)
              .WithErrorMessage("Senha é obrigatória");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("12345")]
    public void Senha_deve_ter_no_minimo_6_caracteres(string senhaCurta)
    {
        var model = Valid();
        model.Senha = senhaCurta;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Senha)
              .WithErrorMessage("Senha deve ter no mínimo 6 caracteres.");
    }

    [Fact]
    public void Senha_valida_nao_deve_gerar_erro()
    {
        var model = Valid();
        model.Senha = "abcdef";
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Senha);
    }

    [Fact]
    public void TipoUsuario_invalido_deve_falhar()
    {
        var model = Valid();
        model.TipoUsuario = (TipoUsuario)42;
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TipoUsuario)
              .WithErrorMessage("Tipo de usuário inválido. Use 'Administrador' ou 'Vendedor'.");
    }

    [Theory]
    [InlineData(TipoUsuario.Administrador)]
    [InlineData(TipoUsuario.Vendedor)]
    public void TipoUsuario_valido_nao_deve_gerar_erro(TipoUsuario tipo)
    {
        var model = Valid();
        model.TipoUsuario = tipo;
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.TipoUsuario);
    }
}