using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWay.EF;
using MyWay.EF.DependencyInjection;
using Npgsql;

namespace MyWay.IntegrationTests;

public sealed class PostgresIntegrationTestFixture : IAsyncLifetime
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=myway_integration_tests;Username=myway;Password=myway_password;Include Error Detail=true";

    public string ConnectionString { get; } = BuildConnectionString();

    public DbContextOptions<MyWayDbContext> Options { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        ValidateTestDatabaseName(ConnectionString);

        Options = new DbContextOptionsBuilder<MyWayDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        ValidateTestDatabaseName(ConnectionString);

        await using var dbContext = CreateDbContext();
        await dbContext.Database.EnsureDeletedAsync();
    }

    public MyWayDbContext CreateDbContext()
    {
        return new MyWayDbContext(Options);
    }

    public ServiceProvider CreateServiceProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MyWay"] = ConnectionString
            })
            .Build();

        var services = new ServiceCollection();
        services.AddMyWayEf(configuration);

        return services.BuildServiceProvider();
    }

    public static string BuildConnectionString()
    {
        return Environment.GetEnvironmentVariable("MYWAY_TEST_CONNECTION_STRING")
            ?? DefaultConnectionString;
    }

    private static void ValidateTestDatabaseName(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);
        var databaseName = builder.Database;

        if (string.IsNullOrWhiteSpace(databaseName)
            || (!databaseName.Contains("test", StringComparison.OrdinalIgnoreCase)
                && !databaseName.Contains("integration", StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException(
                "Integration tests can only delete databases whose name contains 'test' or 'integration'.");
        }
    }
}
