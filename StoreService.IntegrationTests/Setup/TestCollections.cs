using Xunit;

namespace StoreService.IntegrationTests.Setup;

// Todos os testes desta collection compartilham o MESMO container SQL
[CollectionDefinition("sqlserver-collection")]
public class SqlServerCollection : ICollectionFixture<SqlServerContainerFixture> { }