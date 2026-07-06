using MyWay.Application.Common;
using MyWay.Application.UseCases.ShipmentOffers;
using MyWay.Core.Shipments;
using MyWay.UnitTests.Common;
using MyWay.UnitTests.TestsSupport.Fakes;

namespace MyWay.UnitTests.Application.UseCases;

public sealed class ShipmentOfferUseCaseTests
{
    [Fact]
    public async Task CreateOffer_ShouldCreateUserCarrierOffer()
    {
        var services = TestServices.Create();
        var request = CreatePublishedRequest(Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        var carrierUserId = Guid.NewGuid();
        var useCase = services.CreateShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(CreateOfferCommand(request.Id, carrierUserId, null));

        Assert.True(result.IsSuccess);
        var offer = Assert.Single(services.ShipmentOffers.Offers);
        Assert.Equal(offer.Id, result.Value);
        Assert.Equal(carrierUserId, offer.CarrierUserId);
        Assert.Null(offer.CarrierCompanyId);
        Assert.Equal(services.CurrentUser.UserId, offer.CreatedByUserId);
        Assert.Equal(services.DateTimeProvider.UtcNow, offer.CreatedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateOffer_ShouldCreateCompanyCarrierOffer()
    {
        var services = TestServices.Create();
        var request = CreatePublishedRequest(Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        var carrierCompanyId = Guid.NewGuid();
        var useCase = services.CreateShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(CreateOfferCommand(request.Id, null, carrierCompanyId));

        Assert.True(result.IsSuccess);
        var offer = Assert.Single(services.ShipmentOffers.Offers);
        Assert.Equal(offer.Id, result.Value);
        Assert.Null(offer.CarrierUserId);
        Assert.Equal(carrierCompanyId, offer.CarrierCompanyId);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateOffer_ShouldReturnNotFound_WhenRequestDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.CreateShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(CreateOfferCommand(Guid.NewGuid(), Guid.NewGuid(), null));

        AssertFailure(result, "ShipmentRequests.NotFound");
        Assert.Empty(services.ShipmentOffers.Offers);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateOffer_ShouldReturnForbidden_WhenUserCannotActAsCarrier()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCarrierResult = false;
        var request = CreatePublishedRequest(Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        var useCase = services.CreateShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(CreateOfferCommand(request.Id, Guid.NewGuid(), null));

        AssertFailure(result, "Access.Forbidden");
        Assert.Empty(services.ShipmentOffers.Offers);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AcceptOffer_ShouldAcceptOfferCreateOrderAndConvertRequest()
    {
        var services = TestServices.Create();
        var request = CreatePublishedRequest(Guid.NewGuid());
        var carrierUserId = Guid.NewGuid();
        var offer = CreateUserCarrierOffer(request.Id, carrierUserId);
        await services.ShipmentRequests.AddAsync(request);
        await services.ShipmentOffers.AddAsync(offer);
        var useCase = services.AcceptShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new AcceptShipmentOfferCommand(request.Id, offer.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOfferStatus.Accepted, offer.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, offer.AcceptedAt);
        Assert.Equal(ShipmentRequestStatus.ConvertedToOrder, request.Status);
        Assert.Equal(offer.Id, request.AcceptedOfferId);
        var order = Assert.Single(services.ShipmentOrders.Orders);
        Assert.Equal(order.Id, result.Value);
        Assert.Equal(request.Id, order.ShipmentRequestId);
        Assert.Equal(offer.Id, order.AcceptedOfferId);
        Assert.Equal(carrierUserId, order.CarrierUserId);
        Assert.Equal(offer.OfferedPrice, order.FinalPrice);
        Assert.Equal(services.DateTimeProvider.UtcNow, order.CreatedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AcceptOffer_ShouldReturnNotFound_WhenRequestDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.AcceptShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new AcceptShipmentOfferCommand(Guid.NewGuid(), Guid.NewGuid()));

        AssertFailure(result, "ShipmentRequests.NotFound");
        Assert.Empty(services.ShipmentOrders.Orders);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AcceptOffer_ShouldReturnNotFound_WhenOfferDoesNotExist()
    {
        var services = TestServices.Create();
        var request = CreatePublishedRequest(Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        var useCase = services.AcceptShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new AcceptShipmentOfferCommand(request.Id, Guid.NewGuid()));

        AssertFailure(result, "ShipmentOffers.NotFound");
        Assert.Empty(services.ShipmentOrders.Orders);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AcceptOffer_ShouldReturnFailure_WhenOfferDoesNotBelongToRequest()
    {
        var services = TestServices.Create();
        var request = CreatePublishedRequest(Guid.NewGuid());
        var offer = CreateUserCarrierOffer(Guid.NewGuid(), Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        await services.ShipmentOffers.AddAsync(offer);
        var useCase = services.AcceptShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new AcceptShipmentOfferCommand(request.Id, offer.Id));

        AssertFailure(result, "ShipmentOffers.DoesNotBelongToRequest");
        Assert.Equal(ShipmentOfferStatus.Submitted, offer.Status);
        Assert.Empty(services.ShipmentOrders.Orders);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AcceptOffer_ShouldReturnForbidden_WhenUserCannotActAsCustomer()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCustomerResult = false;
        var request = CreatePublishedRequest(Guid.NewGuid());
        var offer = CreateUserCarrierOffer(request.Id, Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        await services.ShipmentOffers.AddAsync(offer);
        var useCase = services.AcceptShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new AcceptShipmentOfferCommand(request.Id, offer.Id));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(ShipmentOfferStatus.Submitted, offer.Status);
        Assert.Equal(ShipmentRequestStatus.Published, request.Status);
        Assert.Empty(services.ShipmentOrders.Orders);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task RejectOffer_ShouldRejectOffer()
    {
        var services = TestServices.Create();
        var request = CreatePublishedRequest(Guid.NewGuid());
        var offer = CreateUserCarrierOffer(request.Id, Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        await services.ShipmentOffers.AddAsync(offer);
        var useCase = services.RejectShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new RejectShipmentOfferCommand(request.Id, offer.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOfferStatus.Rejected, offer.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, offer.RejectedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task RejectOffer_ShouldReturnFailure_WhenOfferDoesNotBelongToRequest()
    {
        var services = TestServices.Create();
        var request = CreatePublishedRequest(Guid.NewGuid());
        var offer = CreateUserCarrierOffer(Guid.NewGuid(), Guid.NewGuid());
        await services.ShipmentRequests.AddAsync(request);
        await services.ShipmentOffers.AddAsync(offer);
        var useCase = services.RejectShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new RejectShipmentOfferCommand(request.Id, offer.Id));

        AssertFailure(result, "ShipmentOffers.DoesNotBelongToRequest");
        Assert.Equal(ShipmentOfferStatus.Submitted, offer.Status);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task WithdrawOffer_ShouldWithdrawOffer()
    {
        var services = TestServices.Create();
        var offer = CreateUserCarrierOffer(Guid.NewGuid(), Guid.NewGuid());
        await services.ShipmentOffers.AddAsync(offer);
        var useCase = services.WithdrawShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new WithdrawShipmentOfferCommand(offer.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOfferStatus.Withdrawn, offer.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, offer.WithdrawnAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task WithdrawOffer_ShouldReturnForbidden_WhenUserCannotActAsCarrier()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCarrierResult = false;
        var offer = CreateUserCarrierOffer(Guid.NewGuid(), Guid.NewGuid());
        await services.ShipmentOffers.AddAsync(offer);
        var useCase = services.WithdrawShipmentOfferUseCase();

        var result = await useCase.ExecuteAsync(new WithdrawShipmentOfferCommand(offer.Id));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(ShipmentOfferStatus.Submitted, offer.Status);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    private static CreateShipmentOfferCommand CreateOfferCommand(
        Guid shipmentRequestId,
        Guid? carrierUserId,
        Guid? carrierCompanyId)
    {
        return new CreateShipmentOfferCommand(
            shipmentRequestId,
            carrierUserId,
            carrierCompanyId,
            DomainTestData.CreateMoney(),
            "Can deliver");
    }

    private static ShipmentRequest CreatePublishedRequest(Guid customerUserId)
    {
        var request = ShipmentRequest.CreatePublic(
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

        request.Publish(DomainTestData.ChangedAt);

        return request;
    }

    private static ShipmentOffer CreateUserCarrierOffer(Guid shipmentRequestId, Guid carrierUserId)
    {
        return ShipmentOffer.CreateForUserCarrier(
            shipmentRequestId,
            carrierUserId,
            Guid.NewGuid(),
            DomainTestData.CreateMoney(),
            DomainTestData.CreatedAt);
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

        public FakeUnitOfWork UnitOfWork { get; } = new();

        public FakeDateTimeProvider DateTimeProvider { get; } = new();

        public static TestServices Create()
        {
            return new TestServices();
        }

        public CreateShipmentOfferUseCase CreateShipmentOfferUseCase()
        {
            return new CreateShipmentOfferUseCase(
                CurrentUser,
                UserAccess,
                ShipmentRequests,
                ShipmentOffers,
                UnitOfWork,
                DateTimeProvider);
        }

        public AcceptShipmentOfferUseCase AcceptShipmentOfferUseCase()
        {
            return new AcceptShipmentOfferUseCase(
                CurrentUser,
                UserAccess,
                ShipmentRequests,
                ShipmentOffers,
                ShipmentOrders,
                UnitOfWork,
                DateTimeProvider);
        }

        public RejectShipmentOfferUseCase RejectShipmentOfferUseCase()
        {
            return new RejectShipmentOfferUseCase(
                CurrentUser,
                UserAccess,
                ShipmentRequests,
                ShipmentOffers,
                UnitOfWork,
                DateTimeProvider);
        }

        public WithdrawShipmentOfferUseCase WithdrawShipmentOfferUseCase()
        {
            return new WithdrawShipmentOfferUseCase(
                CurrentUser,
                UserAccess,
                ShipmentOffers,
                UnitOfWork,
                DateTimeProvider);
        }
    }
}
