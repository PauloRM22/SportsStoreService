using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using StoreService.IntegrationTests.Helpers;
using StoreService.IntegrationTests.Setup;
using Xunit;

namespace StoreService.IntegrationTests.Controllers;

[Collection("sqlserver-collection")]
public class EndToEndFlowTests
{
    private readonly SqlServerContainerFixture _db;
    public EndToEndFlowTests(SqlServerContainerFixture db) => _db = db;

    [Fact]
    public async Task Should_Flow_Admin_Produto_Estoque_Pedido_Confirmar()
    {
        using var factory = new CustomWebApplicationFactory(_db.ConnectionString);
        var client = factory.CreateClient();

        // 1) Bootstrap + Login as Admin
        var adminToken = await client.BootstrapAdminAndLoginAsync();
        client.SetBearer(adminToken);

        // 2) Create product (AdminOnly)
        var createProd = new { Nome = "Camisa Azul", Descricao = "Tam M", Preco = 99.90m };
        var prodResp = await client.PostAsJsonAsync("/api/produtos", createProd);
        prodResp.EnsureSuccessStatusCode();
        var prodJson = await prodResp.Content.ReadAsStringAsync();
        var prodId = JsonDocument.Parse(prodJson).RootElement.GetProperty("id").GetGuid();

        // 3) Registrar entrada de estoque (AdminOnly)
        var entrada = new { ProdutoId = prodId, Quantidade = 10, NumeroNotaFiscal = "NF-1001", Observacao = "Estoque inicial" };
        var estResp = await client.PostAsJsonAsync("/api/estoque/entrada", entrada);
        estResp.EnsureSuccessStatusCode();

        // Verifica saldo 10
        var saldoResp = await client.GetAsync($"/api/estoque/saldo/{prodId}");
        saldoResp.EnsureSuccessStatusCode();
        var saldoJson = JsonDocument.Parse(await saldoResp.Content.ReadAsStringAsync());
        Assert.Equal(10, saldoJson.RootElement.GetProperty("quantidadeAtual").GetInt32());

        // 4) Criar usuário vendedor + login vendedor
        var (vendorToken, vendorId) = await client.CreateVendorAndLoginAsync(adminToken);

        // 5) Criar pedido com 3 itens do produto (VendedorOnly)
        client.SetBearer(vendorToken);
        var pedidoReq = new
        {
            ClienteDocumento = "123.456.789-00",
            VendedorNome = "Vendedor 1",
            Itens = new[] { new { ProdutoId = prodId, Quantidade = 3 } }
        };
        var pedResp = await client.PostAsJsonAsync("/api/pedidos", pedidoReq);
        pedResp.EnsureSuccessStatusCode();
        var pedJson = JsonDocument.Parse(await pedResp.Content.ReadAsStringAsync());
        var pedidoId = pedJson.RootElement.GetProperty("id").GetGuid();
        Assert.Equal("Rascunho", pedJson.RootElement.GetProperty("status").GetString());

        // 6) Confirmar pedido (VendedorOnly)
        var confResp = await client.PostAsync($"/api/pedidos/{pedidoId}/confirmar", content: null);
        confResp.EnsureSuccessStatusCode();
        var confJson = JsonDocument.Parse(await confResp.Content.ReadAsStringAsync());
        Assert.Equal("Confirmado", confJson.RootElement.GetProperty("status").GetString());
        Assert.Equal(99.90m * 3, confJson.RootElement.GetProperty("valorTotal").GetDecimal(), 2);

        // 7) Saldo deve cair para 7
        client.SetBearer(adminToken);
        var saldo2Resp = await client.GetAsync($"/api/estoque/saldo/{prodId}");
        saldo2Resp.EnsureSuccessStatusCode();
        var saldo2Json = JsonDocument.Parse(await saldo2Resp.Content.ReadAsStringAsync());
        Assert.Equal(7, saldo2Json.RootElement.GetProperty("quantidadeAtual").GetInt32());
    }

    [Fact]
    public async Task Should_Reject_Vendedor_On_AdminOnly_Endpoints()
    {
        using var factory = new CustomWebApplicationFactory(_db.ConnectionString);
        var client = factory.CreateClient();

        var adminToken = await client.BootstrapAdminAndLoginAsync();
        var (vendorToken, _) = await client.CreateVendorAndLoginAsync(adminToken);

        // Tenta acessar /api/produtos (AdminOnly) com token de vendedor -> 403
        client.SetBearer(vendorToken);
        var resp = await client.GetAsync("/api/produtos");
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }
}
