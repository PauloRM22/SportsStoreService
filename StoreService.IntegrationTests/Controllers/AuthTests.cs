using StoreService.IntegrationTests.Setup;
using System.Net;
using System.Net.Http.Json;

namespace StoreService.IntegrationTests.Controllers;

[Collection("sqlserver-collection")]
public class AuthTests
{
    private readonly SqlServerContainerFixture _db;
    public AuthTests(SqlServerContainerFixture db) => _db = db;

    [Fact]
    public async Task Deve_fazer_bootstrap_e_login()
    {
        using var factory = new CustomWebApplicationFactory(_db.ConnectionString);
        var client = factory.CreateClient();

        var bootstrap = new { Nome = "Admin QA", Email = "qa@test.local", Senha = "Q@123456", TipoUsuario = "Administrador" };
        var r = await client.PostAsJsonAsync("/api/usuarios/bootstrap", bootstrap);
        Assert.True(r.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.Forbidden);

        var login = await client.PostAsJsonAsync("/api/Auth/login",
            new { Email = bootstrap.Email, Senha = bootstrap.Senha });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }
}