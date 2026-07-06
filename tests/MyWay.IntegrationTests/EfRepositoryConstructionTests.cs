using Microsoft.EntityFrameworkCore;
using MyWay.EF;
using MyWay.EF.Repositories;

namespace MyWay.IntegrationTests;

public sealed class EfRepositoryConstructionTests
{
    [Fact]
    public void Can_construct_all_ef_repositories()
    {
        var options = new DbContextOptionsBuilder<MyWayDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway")
            .Options;

        using var dbContext = new MyWayDbContext(options);

        Assert.NotNull(new EfUserRepository(dbContext));
        Assert.NotNull(new EfCompanyRepository(dbContext));
        Assert.NotNull(new EfCompanyMemberRepository(dbContext));
        Assert.NotNull(new EfCustomerProfileRepository(dbContext));
        Assert.NotNull(new EfCarrierProfileRepository(dbContext));
        Assert.NotNull(new EfDriverProfileRepository(dbContext));
        Assert.NotNull(new EfVehicleRepository(dbContext));
        Assert.NotNull(new EfWarehouseRepository(dbContext));
        Assert.NotNull(new EfShipmentRequestRepository(dbContext));
        Assert.NotNull(new EfShipmentOfferRepository(dbContext));
        Assert.NotNull(new EfShipmentOrderRepository(dbContext));
        Assert.NotNull(new EfCarrierListingRepository(dbContext));
        Assert.NotNull(new EfResourceReservationRepository(dbContext));
        Assert.NotNull(new EfWaybillRepository(dbContext));
        Assert.NotNull(new EfReviewRepository(dbContext));
        Assert.NotNull(new EfUnitOfWork(dbContext));
    }
}
