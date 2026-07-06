using MyWay.Application.Common;
using MyWay.Application.UseCases.ShipmentRequests;
using MyWay.Core.CarrierListings;
using MyWay.Core.Shipments;
using MyWay.UnitTests.Common;
using MyWay.UnitTests.TestsSupport.Fakes;

namespace MyWay.UnitTests.Application.UseCases;

public sealed class ShipmentRequestUseCaseTests
{
    [Fact]
    public async Task CreatePublic_ShouldCreateRequestAndReturnId()
    {
        var services = TestServices.Create();
        var customerUserId = Guid.NewGuid();
        var useCase = services.CreatePublicShipmentRequestUseCase();
        var command = CreatePublicCommand(customerUserId);

        var result = await useCase.ExecuteAsync(command);

        Assert.True(result.IsSuccess);
        var request = Assert.Single(services.ShipmentRequests.Requests);
        Assert.Equal(request.Id, result.Value);
        Assert.Equal(ShipmentRequestType.Public, request.Type);
        Assert.Equal(services.CurrentUser.UserId, request.CreatedByUserId);
        Assert.Equal(services.DateTimeProvider.UtcNow, request.CreatedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreatePublic_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        var services = TestServices.Create();
        services.CurrentUser.IsAuthenticated = false;
        var useCase = services.CreatePublicShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(CreatePublicCommand(Guid.NewGuid()));

        AssertFailure(result, "Common.Unauthorized");
        Assert.Empty(services.ShipmentRequests.Requests);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreatePublic_ShouldReturnForbidden_WhenUserCannotActAsCustomer()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCustomerResult = false;
        var useCase = services.CreatePublicShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(CreatePublicCommand(Guid.NewGuid()));

        AssertFailure(result, "Access.Forbidden");
        Assert.Empty(services.ShipmentRequests.Requests);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreatePublic_ShouldCallUnitOfWork()
    {
        var services = TestServices.Create();
        var useCase = services.CreatePublicShipmentRequestUseCase();

        await useCase.ExecuteAsync(CreatePublicCommand(Guid.NewGuid()));

        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateDirect_ShouldCreateRequestToAvailableCarrierListing()
    {
        var services = TestServices.Create();
        var listing = CreateAvailableListing();
        await services.CarrierListings.AddAsync(listing);
        var useCase = services.CreateDirectShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(CreateDirectCommand(Guid.NewGuid(), listing.Id));

        Assert.True(result.IsSuccess);
        var request = Assert.Single(services.ShipmentRequests.Requests);
        Assert.Equal(request.Id, result.Value);
        Assert.Equal(ShipmentRequestType.DirectToCarrier, request.Type);
        Assert.Equal(listing.Id, request.TargetCarrierListingId);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateDirect_ShouldReturnNotFound_WhenCarrierListingDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.CreateDirectShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(CreateDirectCommand(Guid.NewGuid(), Guid.NewGuid()));

        AssertFailure(result, "CarrierListings.NotFound");
        Assert.Empty(services.ShipmentRequests.Requests);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateDirect_ShouldReturnFailure_WhenCarrierListingIsNotAvailable()
    {
        var services = TestServices.Create();
        var listing = CarrierListing.CreateForUserCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Draft listing",
            DomainTestData.CreatedAt);
        await services.CarrierListings.AddAsync(listing);
        var useCase = services.CreateDirectShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(CreateDirectCommand(Guid.NewGuid(), listing.Id));

        AssertFailure(result, "CarrierListings.NotAvailable");
        Assert.Empty(services.ShipmentRequests.Requests);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Publish_ShouldPublishRequest()
    {
        var services = TestServices.Create();
        var request = CreateDraftRequest(Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        var useCase = services.PublishShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(new PublishShipmentRequestCommand(request.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentRequestStatus.Published, request.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, request.PublishedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Publish_ShouldReturnNotFound_WhenRequestDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.PublishShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(new PublishShipmentRequestCommand(Guid.NewGuid()));

        AssertFailure(result, "ShipmentRequests.NotFound");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Publish_ShouldReturnForbidden_WhenUserCannotActAsCustomer()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCustomerResult = false;
        var request = CreateDraftRequest(Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        var useCase = services.PublishShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(new PublishShipmentRequestCommand(request.Id));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(ShipmentRequestStatus.Draft, request.Status);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Cancel_ShouldCancelRequest()
    {
        var services = TestServices.Create();
        var request = CreateDraftRequest(Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        var useCase = services.CancelShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(new CancelShipmentRequestCommand(request.Id, "No longer needed"));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentRequestStatus.Cancelled, request.Status);
        Assert.Equal("No longer needed", request.CancellationReason);
        Assert.Equal(services.DateTimeProvider.UtcNow, request.CancelledAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Cancel_ShouldReturnNotFound_WhenRequestDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.CancelShipmentRequestUseCase();

        var result = await useCase.ExecuteAsync(new CancelShipmentRequestCommand(Guid.NewGuid(), null));

        AssertFailure(result, "ShipmentRequests.NotFound");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    private static CreatePublicShipmentRequestCommand CreatePublicCommand(Guid customerUserId)
    {
        return new CreatePublicShipmentRequestCommand(
            customerUserId,
            null,
            DomainTestData.CreateAddress("Pickup"),
            DomainTestData.CreateAddress("Delivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreateMoney(),
            DomainTestData.PlannedPickupAt,
            DomainTestData.PlannedDeliveryAt);
    }

    private static CreateDirectShipmentRequestCommand CreateDirectCommand(Guid customerUserId, Guid carrierListingId)
    {
        return new CreateDirectShipmentRequestCommand(
            customerUserId,
            null,
            carrierListingId,
            DomainTestData.CreateAddress("Pickup"),
            DomainTestData.CreateAddress("Delivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreateMoney(),
            DomainTestData.PlannedPickupAt,
            DomainTestData.PlannedDeliveryAt);
    }

    private static ShipmentRequest CreateDraftRequest(Guid customerUserId)
    {
        return ShipmentRequest.CreatePublic(
            Guid.NewGuid(),
            customerUserId,
            null,
            DomainTestData.CreateAddress("Pickup"),
            DomainTestData.CreateAddress("Delivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreateMoney(),
            DomainTestData.PlannedPickupAt,
            DomainTestData.PlannedDeliveryAt,
            DomainTestData.CreatedAt);
    }

    private static CarrierListing CreateAvailableListing()
    {
        var listing = CarrierListing.CreateForUserCarrier(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Available truck",
            DomainTestData.CreatedAt);

        listing.SetAvailable(DomainTestData.ChangedAt);

        return listing;
    }

    private static void AssertFailure(Result result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(expectedCode, result.Error?.Code);
    }

    private static void AssertFailure<T>(Result<T> result, string expectedCode)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(expectedCode, result.Error?.Code);
    }

    private sealed class TestServices
    {
        private TestServices()
        {
        }

        public FakeCurrentUserContext CurrentUser { get; } = new();

        public FakeUserAccessService UserAccess { get; } = new();

        public InMemoryShipmentRequestRepository ShipmentRequests { get; } = new();

        public InMemoryShipmentOfferRepository ShipmentOffers { get; } = new();

        public InMemoryShipmentOrderRepository ShipmentOrders { get; } = new();

        public InMemoryCarrierListingRepository CarrierListings { get; } = new();

        public FakeUnitOfWork UnitOfWork { get; } = new();

        public FakeDateTimeProvider DateTimeProvider { get; } = new();

        public static TestServices Create()
        {
            return new TestServices();
        }

        public CreatePublicShipmentRequestUseCase CreatePublicShipmentRequestUseCase()
        {
            return new CreatePublicShipmentRequestUseCase(
                CurrentUser,
                UserAccess,
                ShipmentRequests,
                UnitOfWork,
                DateTimeProvider);
        }

        public CreateDirectShipmentRequestUseCase CreateDirectShipmentRequestUseCase()
        {
            return new CreateDirectShipmentRequestUseCase(
                CurrentUser,
                UserAccess,
                ShipmentRequests,
                CarrierListings,
                UnitOfWork,
                DateTimeProvider);
        }

        public PublishShipmentRequestUseCase PublishShipmentRequestUseCase()
        {
            return new PublishShipmentRequestUseCase(
                CurrentUser,
                UserAccess,
                ShipmentRequests,
                UnitOfWork,
                DateTimeProvider);
        }

        public CancelShipmentRequestUseCase CancelShipmentRequestUseCase()
        {
            return new CancelShipmentRequestUseCase(
                CurrentUser,
                UserAccess,
                ShipmentRequests,
                UnitOfWork,
                DateTimeProvider);
        }
    }
}
