namespace StoreService.IntegrationTests.Setup;

[CollectionDefinition("sqlserver-collection")]
public class SqlServerCollection : ICollectionFixture<SqlServerContainerFixture> { }