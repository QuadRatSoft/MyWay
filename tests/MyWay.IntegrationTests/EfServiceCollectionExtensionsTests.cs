using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWay.Application.Abstractions;
using MyWay.EF;
using MyWay.EF.DependencyInjection;

namespace MyWay.IntegrationTests;

public sealed class EfServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMyWayEf_ShouldRegisterDbContextAndUnitOfWork()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MyWay"] =
                    "Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddMyWayEf(configuration);

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<MyWayDbContext>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUnitOfWork>());
    }

    [Fact]
    public void AddMyWayEf_ShouldThrow_WhenConnectionStringMissing()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() => services.AddMyWayEf(configuration));
    }
}
