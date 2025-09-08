using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;                // ADICIONE
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SportsStoreService;
using StoreService.Infrastructure.Data;

namespace StoreService.IntegrationTests.Setup;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _serverConn;   // conecta na master
    private readonly string _dbName;       // nome do banco da rodada
    private readonly string _dbConn;       // connection string final (com Initial Catalog)

    public CustomWebApplicationFactory(string serverConnectionString)
    {
        _dbName = $"SportsStore_{Guid.NewGuid():N}";

        // monta conn para master
        var csbMaster = new SqlConnectionStringBuilder(serverConnectionString)
        {
            InitialCatalog = "master",
            TrustServerCertificate = true,
            MultipleActiveResultSets = true
        };
        _serverConn = csbMaster.ToString();

        // monta conn para o DB da rodada
        var csbDb = new SqlConnectionStringBuilder(serverConnectionString)
        {
            InitialCatalog = _dbName,
            TrustServerCertificate = true,
            MultipleActiveResultSets = true
        };
        _dbConn = csbDb.ToString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development"); // libera bootstrap

        builder.ConfigureServices(services =>
        {
            // 1) Criar o banco da rodada na master (se não existir)
            using (var conn = new SqlConnection(_serverConn))
            {
                conn.Open();
                using var cmd = new SqlCommand(
                    $"IF DB_ID('{_dbName}') IS NULL CREATE DATABASE [{_dbName}];", conn);
                cmd.ExecuteNonQuery();
            }

            // 2) Remover registro antigo do AppDbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            // 3) Registrar AppDbContext apontando para o banco da rodada
            services.AddDbContext<AppDbContext>(opts => opts.UseSqlServer(_dbConn));

            // 4) Aplicar migrações (com pequena espera ativa)
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var limite = DateTime.UtcNow.AddSeconds(90);
            while (!db.Database.CanConnect())
            {
                if (DateTime.UtcNow > limite)
                    throw new TimeoutException("SQL Server não ficou pronto a tempo.");
                Thread.Sleep(1000);
            }

            // ⚠️ NADA de EnsureDeleted() aqui!
            db.Database.Migrate();
        });
    }

    protected override IHost CreateHost(IHostBuilder builder) => base.CreateHost(builder);
}