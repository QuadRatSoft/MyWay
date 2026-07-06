using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.EF;
using MyWay.EF.DependencyInjection;

namespace MyWay.IntegrationTests;

public sealed class EfServiceCollectionExtensionsTests
{
    [Fact]
    public void AddMyWayEf_ShouldRegisterDbContextUnitOfWorkAndRepositories()
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
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUserRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICompanyRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICompanyMemberRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICustomerProfileRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICarrierProfileRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IDriverProfileRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IVehicleRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IWarehouseRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IShipmentRequestRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IShipmentOfferRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IShipmentOrderRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICarrierListingRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IResourceReservationRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IWaybillRepository>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IReviewRepository>());
    }

    [Fact]
    public void AddMyWayEf_ShouldThrow_WhenConnectionStringMissing()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() => services.AddMyWayEf(configuration));
    }
}
