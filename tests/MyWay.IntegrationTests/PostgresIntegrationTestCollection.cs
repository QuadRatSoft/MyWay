namespace MyWay.IntegrationTests;

[CollectionDefinition(nameof(PostgresIntegrationTestCollection), DisableParallelization = true)]
public sealed class PostgresIntegrationTestCollection
    : ICollectionFixture<PostgresIntegrationTestFixture>
{
}
