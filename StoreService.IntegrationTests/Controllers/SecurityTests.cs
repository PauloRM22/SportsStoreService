using System.Net;
using StoreService.IntegrationTests.Setup;
using Xunit;

namespace StoreService.IntegrationTests.Controllers;

[Collection("sqlserver-collection")]
public class SecurityTests
{
    private readonly SqlServerContainerFixture _db;
    public SecurityTests(SqlServerContainerFixture db) => _db = db;

    [Fact]
    public async Task Deve_retornar_401_sem_token()
    {
        using var factory = new CustomWebApplicationFactory(_db.ConnectionString);
        var client = factory.CreateClient();

        var resp = await client.GetAsync("/api/produtos");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
