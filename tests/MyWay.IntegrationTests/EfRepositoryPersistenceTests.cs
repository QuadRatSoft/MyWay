using MyWay.Application.Common.Pagination;
using MyWay.Application.UseCases.Boards;
using MyWay.Core.CarrierListings;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Resources;
using MyWay.Core.Shipments;
using MyWay.Core.Users;
using MyWay.EF;
using MyWay.EF.Repositories;

namespace MyWay.IntegrationTests;

[Collection(nameof(PostgresIntegrationTestCollection))]
public sealed class EfRepositoryPersistenceTests
{
    private static readonly DateTimeOffset CreatedAt =
        new(2026, 1, 1, 10, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset LaterAt =
        new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    private readonly PostgresIntegrationTestFixture fixture;

    public EfRepositoryPersistenceTests(PostgresIntegrationTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task UserRepository_ShouldPersistAndLoadUser()
    {
        var authUserId = Guid.NewGuid();
        var user = User.Create(
            authUserId,
            "driver@example.com",
            "Driver One",
            CreatedAt,
            "+10000000000");

        await using (var writeContext = fixture.CreateDbContext())
        {
            var repository = new EfUserRepository(writeContext);
            var unitOfWork = new EfUnitOfWork(writeContext);

            await repository.AddAsync(user);
            await unitOfWork.SaveChangesAsync();
        }

        await using var readContext = fixture.CreateDbContext();
        var readRepository = new EfUserRepository(readContext);

        var byId = await readRepository.GetByIdAsync(user.Id);
        var byAuthUserId = await readRepository.GetByAuthUserIdAsync(authUserId);
        var byEmail = await readRepository.GetByEmailAsync("DRIVER@EXAMPLE.COM");

        Assert.NotNull(byId);
        Assert.NotNull(byAuthUserId);
        Assert.NotNull(byEmail);
        Assert.Equal(user.Id, byId.Id);
        Assert.Equal(user.Id, byAuthUserId.Id);
        Assert.Equal(user.Id, byEmail.Id);
        Assert.Equal("driver@example.com", byEmail.Email);
    }

    [Fact]
    public async Task ShipmentRequestRepository_ShouldSearchPublishedRequests()
    {
        var publishedRequest = CreateShipmentRequest(
            "Board Pickup City",
            "Board Delivery City",
            new Money(1500m, "USD"));
        publishedRequest.Publish(LaterAt);

        var draftRequest = CreateShipmentRequest(
            "Board Pickup City",
            "Board Delivery City",
            new Money(1500m, "USD"));

        await using (var writeContext = fixture.CreateDbContext())
        {
            var repository = new EfShipmentRequestRepository(writeContext);
            var unitOfWork = new EfUnitOfWork(writeContext);

            await repository.AddAsync(publishedRequest);
            await repository.AddAsync(draftRequest);
            await unitOfWork.SaveChangesAsync();
        }

        await using var readContext = fixture.CreateDbContext();
        var readRepository = new EfShipmentRequestRepository(readContext);

        var result = await readRepository.SearchPublishedForBoardAsync(
            new ShipmentRequestBoardFilter(
                PickupCity: "board pickup city",
                DeliveryCity: "BOARD DELIVERY CITY",
                PickupFrom: new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero),
                PickupTo: new DateTimeOffset(2026, 1, 2, 23, 59, 0, TimeSpan.Zero),
                MinPriceAmount: 1000m,
                MaxPriceAmount: 2000m,
                Currency: "usd"),
            new PaginationRequest(1, 10));

        Assert.Contains(result.Items, request => request.Id == publishedRequest.Id);
        Assert.DoesNotContain(result.Items, request => request.Id == draftRequest.Id);
    }

    [Fact]
    public async Task CarrierListingRepository_ShouldSearchAvailableListings()
    {
        var availableListing = CarrierListing.CreateForUserCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Available carrier",
            CreatedAt,
            basePrice: new Money(900m, "USD"));
        availableListing.SetAvailable(LaterAt);

        var draftListing = CarrierListing.CreateForUserCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Draft carrier",
            CreatedAt,
            basePrice: new Money(900m, "USD"));

        await using (var writeContext = fixture.CreateDbContext())
        {
            var repository = new EfCarrierListingRepository(writeContext);
            var unitOfWork = new EfUnitOfWork(writeContext);

            await repository.AddAsync(availableListing);
            await repository.AddAsync(draftListing);
            await unitOfWork.SaveChangesAsync();
        }

        await using var readContext = fixture.CreateDbContext();
        var readRepository = new EfCarrierListingRepository(readContext);

        var result = await readRepository.SearchVisibleForBoardAsync(
            new CarrierListingBoardFilter(
                Region: null,
                RouteFrom: null,
                RouteTo: null,
                MinPriceAmount: 800m,
                MaxPriceAmount: 1000m,
                Currency: "usd"),
            new PaginationRequest(1, 10));

        Assert.Contains(result.Items, listing => listing.Id == availableListing.Id);
        Assert.DoesNotContain(result.Items, listing => listing.Id == draftListing.Id);
    }

    [Fact]
    public async Task ResourceReservationRepository_ShouldReturnOnlyActiveReservations()
    {
        var shipmentOrderId = Guid.NewGuid();
        var driverUserId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();

        var activeReservation = ResourceReservation.Create(
            shipmentOrderId,
            driverUserId,
            vehicleId,
            new DateRange(
                new DateTimeOffset(2026, 1, 4, 10, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 1, 4, 12, 0, 0, TimeSpan.Zero)),
            CreatedAt);

        var cancelledReservation = ResourceReservation.Create(
            shipmentOrderId,
            driverUserId,
            vehicleId,
            new DateRange(
                new DateTimeOffset(2026, 1, 5, 10, 0, 0, TimeSpan.Zero),
                new DateTimeOffset(2026, 1, 5, 12, 0, 0, TimeSpan.Zero)),
            CreatedAt);
        cancelledReservation.Cancel(LaterAt);

        await using (var writeContext = fixture.CreateDbContext())
        {
            var repository = new EfResourceReservationRepository(writeContext);
            var unitOfWork = new EfUnitOfWork(writeContext);

            await repository.AddAsync(activeReservation);
            await repository.AddAsync(cancelledReservation);
            await unitOfWork.SaveChangesAsync();
        }

        await using var readContext = fixture.CreateDbContext();
        var readRepository = new EfResourceReservationRepository(readContext);

        var byDriver = await readRepository.GetActiveByDriverUserIdAsync(driverUserId);
        var byVehicle = await readRepository.GetActiveByVehicleIdAsync(vehicleId);
        var byOrder = await readRepository.GetActiveByShipmentOrderIdAsync(shipmentOrderId);

        Assert.Single(byDriver);
        Assert.Single(byVehicle);
        Assert.Single(byOrder);
        Assert.Equal(activeReservation.Id, byDriver.Single().Id);
        Assert.Equal(activeReservation.Id, byVehicle.Single().Id);
        Assert.Equal(activeReservation.Id, byOrder.Single().Id);
    }

    private static ShipmentRequest CreateShipmentRequest(
        string pickupCity,
        string deliveryCity,
        Money customerPrice)
    {
        return ShipmentRequest.CreatePublic(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            new Address(
                country: "US",
                region: "CA",
                city: pickupCity,
                street: "Pickup Street",
                house: "1"),
            new Address(
                country: "US",
                region: "NV",
                city: deliveryCity,
                street: "Delivery Street",
                house: "2"),
            new CargoDetails(
                name: "Electronics",
                weightKg: 120m,
                description: "Packed electronics",
                volumeM3: 4m),
            customerPrice,
            new DateTimeOffset(2026, 1, 2, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 1, 3, 10, 0, 0, TimeSpan.Zero),
            CreatedAt);
    }
}
