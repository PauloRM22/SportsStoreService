using FluentValidation.TestHelper;
using StoreService.Application.Pedidos.DTOs;
using StoreService.Application.Validations.Pedidos;
using Xunit;

// alias conforme seu DTO real de item:
using PedidoItem = StoreService.Application.Pedidos.DTOs.PedidoItemRequest;

namespace StoreService.Tests.Validators;

public class PedidosValidatorsTests
{
    private readonly CreatePedidoRequestValidator _validator = new();

    private static CreatePedidoRequest Valid()
        => new()
        {
            ClienteDocumento = "12345678901",
            VendedorNome = "Vendedor 1",
            Itens = new List<PedidoItem>
            {
                new() { ProdutoId = Guid.NewGuid(), Quantidade = 2 }
            }
        };

    [Fact]
    public void Documento_cliente_invalido_deve_falhar()
    {
        var m = Valid();
        m.ClienteDocumento = "abc";
        var r = _validator.TestValidate(m);
        r.ShouldHaveValidationErrorFor(x => x.ClienteDocumento);
    }

    [Fact]
    public void Vendedor_nome_obrigatorio()
    {
        var m = Valid();
        m.VendedorNome = "   ";
        var r = _validator.TestValidate(m);
        r.ShouldHaveValidationErrorFor(x => x.VendedorNome);
    }

    [Fact]
    public void Item_sem_produto_ou_qtd_invalida_deve_falhar()
    {
        var m = Valid();
        m.Itens = new List<PedidoItem>
    {
        new() { ProdutoId = Guid.Empty, Quantidade = 1 },
        new() { ProdutoId = Guid.NewGuid(), Quantidade = 0 }
    };

        var r = _validator.TestValidate(m);

        // Aqui TEM que haver erro (é um cenário inválido):
        Assert.False(r.IsValid); // <- mantém essa linha

        r.ShouldHaveValidationErrorFor("Itens[0].ProdutoId");
        r.ShouldHaveValidationErrorFor("Itens[1].Quantidade");
    }

    [Fact]
    public void Pedido_valido_nao_deve_ter_erros()
    {
        var r = _validator.TestValidate(Valid());
        r.ShouldNotHaveAnyValidationErrors();
    }
}
