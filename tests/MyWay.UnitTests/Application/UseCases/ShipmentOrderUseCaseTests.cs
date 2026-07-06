using MyWay.Application.Common;
using MyWay.Application.Services;
using MyWay.Application.UseCases.ShipmentOrders;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Resources;
using MyWay.Core.Shipments;
using MyWay.UnitTests.Common;
using MyWay.UnitTests.TestsSupport.Fakes;

namespace MyWay.UnitTests.Application.UseCases;

public sealed class ShipmentOrderUseCaseTests
{
    [Fact]
    public async Task AssignResources_ShouldAssignDriverAndVehicleCreateReservationAndReturnId()
    {
        var services = TestServices.Create();
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var driverUserId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var useCase = services.AssignResourcesUseCase();

        var result = await useCase.ExecuteAsync(new AssignShipmentOrderResourcesCommand(
            order.Id,
            driverUserId,
            vehicleId));

        Assert.True(result.IsSuccess);
        Assert.Equal(driverUserId, order.AssignedDriverUserId);
        Assert.Equal(vehicleId, order.AssignedVehicleId);
        Assert.Equal(ShipmentOrderStatus.ReadyToStart, order.Status);
        var reservation = Assert.Single(services.Reservations.Reservations);
        Assert.Equal(reservation.Id, result.Value);
        Assert.Equal(order.Id, reservation.ShipmentOrderId);
        Assert.Equal(driverUserId, reservation.DriverUserId);
        Assert.Equal(vehicleId, reservation.VehicleId);
        Assert.Equal(ResourceReservationStatus.Active, reservation.Status);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AssignResources_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        var services = TestServices.Create();
        services.CurrentUser.IsAuthenticated = false;
        var useCase = services.AssignResourcesUseCase();

        var result = await useCase.ExecuteAsync(new AssignShipmentOrderResourcesCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()));

        AssertFailure(result, "Common.Unauthorized");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AssignResources_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.AssignResourcesUseCase();

        var result = await useCase.ExecuteAsync(new AssignShipmentOrderResourcesCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid()));

        AssertFailure(result, "ShipmentOrders.NotFound");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AssignResources_ShouldReturnForbidden_WhenUserCannotActAsCarrier()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCarrierResult = false;
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.AssignResourcesUseCase();

        var result = await useCase.ExecuteAsync(new AssignShipmentOrderResourcesCommand(
            order.Id,
            Guid.NewGuid(),
            Guid.NewGuid()));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AssignResources_ShouldReturnDriverUnavailable_WhenDriverIsBusy()
    {
        var services = TestServices.Create();
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var driverUserId = Guid.NewGuid();
        await services.Reservations.AddAsync(CreateReservation(Guid.NewGuid(), driverUserId, null));
        var useCase = services.AssignResourcesUseCase();

        var result = await useCase.ExecuteAsync(new AssignShipmentOrderResourcesCommand(
            order.Id,
            driverUserId,
            Guid.NewGuid()));

        AssertFailure(result, "ResourceReservations.DriverUnavailable");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AssignResources_ShouldReturnVehicleUnavailable_WhenVehicleIsBusy()
    {
        var services = TestServices.Create();
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var vehicleId = Guid.NewGuid();
        await services.Reservations.AddAsync(CreateReservation(Guid.NewGuid(), null, vehicleId));
        var useCase = services.AssignResourcesUseCase();

        var result = await useCase.ExecuteAsync(new AssignShipmentOrderResourcesCommand(
            order.Id,
            Guid.NewGuid(),
            vehicleId));

        AssertFailure(result, "ResourceReservations.VehicleUnavailable");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task AssignResources_ShouldReturnAlreadyExists_WhenOrderHasActiveReservation()
    {
        var services = TestServices.Create();
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        await services.Reservations.AddAsync(CreateReservation(order.Id, Guid.NewGuid(), Guid.NewGuid()));
        var useCase = services.AssignResourcesUseCase();

        var result = await useCase.ExecuteAsync(new AssignShipmentOrderResourcesCommand(
            order.Id,
            Guid.NewGuid(),
            Guid.NewGuid()));

        AssertFailure(result, "ResourceReservations.AlreadyExistsForOrder");
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Start_ShouldMoveReadyOrderToInProgress()
    {
        var services = TestServices.Create();
        var order = CreateReadyToStartOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.StartUseCase();

        var result = await useCase.ExecuteAsync(new StartShipmentOrderCommand(order.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOrderStatus.InProgress, order.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, order.StartedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Start_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.StartUseCase();

        var result = await useCase.ExecuteAsync(new StartShipmentOrderCommand(Guid.NewGuid()));

        AssertFailure(result, "ShipmentOrders.NotFound");
    }

    [Fact]
    public async Task Start_ShouldReturnForbidden_WhenUserCannotActAsCarrier()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCarrierResult = false;
        var order = CreateReadyToStartOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.StartUseCase();

        var result = await useCase.ExecuteAsync(new StartShipmentOrderCommand(order.Id));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(ShipmentOrderStatus.ReadyToStart, order.Status);
    }

    [Fact]
    public async Task Start_ShouldReturnDomainRuleViolation_WhenOrderIsNotReadyToStart()
    {
        var services = TestServices.Create();
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.StartUseCase();

        var result = await useCase.ExecuteAsync(new StartShipmentOrderCommand(order.Id));

        AssertFailure(result, "Domain.RuleViolation");
        Assert.Equal(ShipmentOrderStatus.Created, order.Status);
    }

    [Fact]
    public async Task MarkDelivered_ShouldMoveInProgressOrderToDeliveredAndCompleteReservation()
    {
        var services = TestServices.Create();
        var order = CreateInProgressOrder();
        await services.Orders.AddAsync(order);
        var reservation = CreateReservation(order.Id, order.AssignedDriverUserId, order.AssignedVehicleId);
        await services.Reservations.AddAsync(reservation);
        var useCase = services.MarkDeliveredUseCase();

        var result = await useCase.ExecuteAsync(new MarkShipmentOrderDeliveredCommand(order.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOrderStatus.Delivered, order.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, order.DeliveredAt);
        Assert.Equal(ResourceReservationStatus.Completed, reservation.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, reservation.CompletedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task MarkDelivered_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.MarkDeliveredUseCase();

        var result = await useCase.ExecuteAsync(new MarkShipmentOrderDeliveredCommand(Guid.NewGuid()));

        AssertFailure(result, "ShipmentOrders.NotFound");
    }

    [Fact]
    public async Task MarkDelivered_ShouldReturnForbidden_WhenUserCannotActAsCarrier()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCarrierResult = false;
        var order = CreateInProgressOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.MarkDeliveredUseCase();

        var result = await useCase.ExecuteAsync(new MarkShipmentOrderDeliveredCommand(order.Id));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(ShipmentOrderStatus.InProgress, order.Status);
    }

    [Fact]
    public async Task MarkDelivered_ShouldReturnDomainRuleViolation_WhenOrderIsNotInProgress()
    {
        var services = TestServices.Create();
        var order = CreateReadyToStartOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.MarkDeliveredUseCase();

        var result = await useCase.ExecuteAsync(new MarkShipmentOrderDeliveredCommand(order.Id));

        AssertFailure(result, "Domain.RuleViolation");
        Assert.Equal(ShipmentOrderStatus.ReadyToStart, order.Status);
    }

    [Fact]
    public async Task Complete_ShouldMoveDeliveredOrderToCompleted()
    {
        var services = TestServices.Create();
        var order = CreateDeliveredOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.CompleteUseCase();

        var result = await useCase.ExecuteAsync(new CompleteShipmentOrderCommand(order.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOrderStatus.Completed, order.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, order.CompletedAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Complete_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var services = TestServices.Create();
        var useCase = services.CompleteUseCase();

        var result = await useCase.ExecuteAsync(new CompleteShipmentOrderCommand(Guid.NewGuid()));

        AssertFailure(result, "ShipmentOrders.NotFound");
    }

    [Fact]
    public async Task Complete_ShouldReturnForbidden_WhenUserCannotActAsCustomer()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCustomerResult = false;
        var order = CreateDeliveredOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.CompleteUseCase();

        var result = await useCase.ExecuteAsync(new CompleteShipmentOrderCommand(order.Id));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(ShipmentOrderStatus.Delivered, order.Status);
    }

    [Fact]
    public async Task Complete_ShouldReturnDomainRuleViolation_WhenOrderIsNotDelivered()
    {
        var services = TestServices.Create();
        var order = CreateInProgressOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.CompleteUseCase();

        var result = await useCase.ExecuteAsync(new CompleteShipmentOrderCommand(order.Id));

        AssertFailure(result, "Domain.RuleViolation");
        Assert.Equal(ShipmentOrderStatus.InProgress, order.Status);
    }

    [Fact]
    public async Task Cancel_ShouldCancelOrderAndActiveReservation()
    {
        var services = TestServices.Create();
        var order = CreateReadyToStartOrder();
        await services.Orders.AddAsync(order);
        var reservation = CreateReservation(order.Id, order.AssignedDriverUserId, order.AssignedVehicleId);
        await services.Reservations.AddAsync(reservation);
        var useCase = services.CancelUseCase();

        var result = await useCase.ExecuteAsync(new CancelShipmentOrderCommand(order.Id, "Changed plans"));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOrderStatus.Cancelled, order.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, order.CancelledAt);
        Assert.Equal("Changed plans", order.CancellationReason);
        Assert.Equal(ResourceReservationStatus.Cancelled, reservation.Status);
        Assert.Equal(services.DateTimeProvider.UtcNow, reservation.CancelledAt);
        Assert.Equal(1, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Cancel_ShouldAllowCustomerToCancelOrder()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCustomerResult = true;
        services.UserAccess.CanActAsCarrierResult = false;
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.CancelUseCase();

        var result = await useCase.ExecuteAsync(new CancelShipmentOrderCommand(order.Id, null));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public async Task Cancel_ShouldAllowCarrierToCancelOrder()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCustomerResult = false;
        services.UserAccess.CanActAsCarrierResult = true;
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.CancelUseCase();

        var result = await useCase.ExecuteAsync(new CancelShipmentOrderCommand(order.Id, null));

        Assert.True(result.IsSuccess);
        Assert.Equal(ShipmentOrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public async Task Cancel_ShouldReturnForbidden_WhenUserIsNeitherCustomerNorCarrier()
    {
        var services = TestServices.Create();
        services.UserAccess.CanActAsCustomerResult = false;
        services.UserAccess.CanActAsCarrierResult = false;
        var order = CreateOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.CancelUseCase();

        var result = await useCase.ExecuteAsync(new CancelShipmentOrderCommand(order.Id, null));

        AssertFailure(result, "Access.Forbidden");
        Assert.Equal(ShipmentOrderStatus.Created, order.Status);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Cancel_ShouldReturnDomainRuleViolation_WhenOrderIsCompleted()
    {
        var services = TestServices.Create();
        var order = CreateCompletedOrder();
        await services.Orders.AddAsync(order);
        var useCase = services.CancelUseCase();

        var result = await useCase.ExecuteAsync(new CancelShipmentOrderCommand(order.Id, "Too late"));

        AssertFailure(result, "Domain.RuleViolation");
        Assert.Equal(ShipmentOrderStatus.Completed, order.Status);
        Assert.Equal(0, services.UnitOfWork.SaveChangesCallCount);
    }

    private static ShipmentOrder CreateOrder()
    {
        return ShipmentOrder.CreateFromAcceptedOffer(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            Guid.NewGuid(),
            null,
            DomainTestData.CreateAddress("Pickup"),
            DomainTestData.CreateAddress("Delivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreateMoney(),
            DomainTestData.PlannedPickupAt,
            DomainTestData.PlannedDeliveryAt,
            DomainTestData.CreatedAt);
    }

    private static ShipmentOrder CreateReadyToStartOrder()
    {
        var order = CreateOrder();

        order.AssignDriver(Guid.NewGuid());
        order.AssignVehicle(Guid.NewGuid());

        return order;
    }

    private static ShipmentOrder CreateInProgressOrder()
    {
        var order = CreateReadyToStartOrder();

        order.Start(DomainTestData.ChangedAt);

        return order;
    }

    private static ShipmentOrder CreateDeliveredOrder()
    {
        var order = CreateInProgressOrder();

        order.MarkDelivered(DomainTestData.ChangedAt);

        return order;
    }

    private static ShipmentOrder CreateCompletedOrder()
    {
        var order = CreateDeliveredOrder();

        order.Complete(DomainTestData.ChangedAt);

        return order;
    }

    private static ResourceReservation CreateReservation(
        Guid shipmentOrderId,
        Guid? driverUserId,
        Guid? vehicleId)
    {
        return ResourceReservation.Create(
            shipmentOrderId,
            driverUserId,
            vehicleId,
            new DateRange(DomainTestData.PlannedPickupAt, DomainTestData.PlannedDeliveryAt),
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
            Availability = new AvailabilityService(Reservations);
        }

        public FakeCurrentUserContext CurrentUser { get; } = new();

        public FakeUserAccessService UserAccess { get; } = new();

        public InMemoryShipmentOrderRepository Orders { get; } = new();

        public InMemoryResourceReservationRepository Reservations { get; } = new();

        public AvailabilityService Availability { get; }

        public FakeUnitOfWork UnitOfWork { get; } = new();

        public FakeDateTimeProvider DateTimeProvider { get; } = new();

        public static TestServices Create()
        {
            return new TestServices();
        }

        public AssignShipmentOrderResourcesUseCase AssignResourcesUseCase()
        {
            return new AssignShipmentOrderResourcesUseCase(
                CurrentUser,
                UserAccess,
                Orders,
                Reservations,
                Availability,
                UnitOfWork,
                DateTimeProvider);
        }

        public StartShipmentOrderUseCase StartUseCase()
        {
            return new StartShipmentOrderUseCase(
                CurrentUser,
                UserAccess,
                Orders,
                UnitOfWork,
                DateTimeProvider);
        }

        public MarkShipmentOrderDeliveredUseCase MarkDeliveredUseCase()
        {
            return new MarkShipmentOrderDeliveredUseCase(
                CurrentUser,
                UserAccess,
                Orders,
                Reservations,
                UnitOfWork,
                DateTimeProvider);
        }

        public CompleteShipmentOrderUseCase CompleteUseCase()
        {
            return new CompleteShipmentOrderUseCase(
                CurrentUser,
                UserAccess,
                Orders,
                UnitOfWork,
                DateTimeProvider);
        }

        public CancelShipmentOrderUseCase CancelUseCase()
        {
            return new CancelShipmentOrderUseCase(
                CurrentUser,
                UserAccess,
                Orders,
                Reservations,
                UnitOfWork,
                DateTimeProvider);
        }
    }
}
