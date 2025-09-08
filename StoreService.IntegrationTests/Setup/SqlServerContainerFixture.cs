using System.Threading.Tasks;
using Testcontainers.MsSql;
using DotNet.Testcontainers.Builders;
using Xunit;

namespace StoreService.IntegrationTests.Setup;

public class SqlServerContainerFixture : IAsyncLifetime
{
    private MsSqlContainer _container = default!;

    public string ConnectionString => _container.GetConnectionString();

    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder()
           .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
           .WithEnvironment("ACCEPT_EULA", "Y")
           .WithEnvironment("MSSQL_PID", "Developer")
           .WithPassword("Strong!Passw0rd")
           .WithWaitStrategy(Wait.ForUnixContainer()
               .UntilPortIsAvailable(1433)
               .UntilMessageIsLogged("SQL Server is now ready for client connections"))
           .Build();

        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }
}