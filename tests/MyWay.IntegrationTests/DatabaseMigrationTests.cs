using Microsoft.EntityFrameworkCore;

namespace MyWay.IntegrationTests;

[Collection(nameof(PostgresIntegrationTestCollection))]
public sealed class DatabaseMigrationTests
{
    private readonly PostgresIntegrationTestFixture fixture;

    public DatabaseMigrationTests(PostgresIntegrationTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task Can_apply_all_migrations_to_postgresql()
    {
        await using var dbContext = fixture.CreateDbContext();

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        Assert.Empty(pendingMigrations);
    }
}
