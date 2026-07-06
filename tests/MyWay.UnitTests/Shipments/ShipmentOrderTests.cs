using MyWay.Core.Common.Exceptions;
using MyWay.Core.Shipments;
using MyWay.UnitTests.Common;

namespace MyWay.UnitTests.Shipments;

public sealed class ShipmentOrderTests
{
    [Fact]
    public void CreateFromAcceptedOffer_ShouldCreateOrderInCreatedStatus()
    {
        var order = CreateOrder();

        Assert.Equal(ShipmentOrderStatus.Created, order.Status);
        Assert.NotEqual(Guid.Empty, order.Id);
    }

    [Fact]
    public void AssignDriver_ShouldMoveOrderToDriverAssigned()
    {
        var order = CreateOrder();
        var driverUserId = Guid.NewGuid();

        order.AssignDriver(driverUserId);

        Assert.Equal(driverUserId, order.AssignedDriverUserId);
        Assert.Equal(ShipmentOrderStatus.DriverAssigned, order.Status);
    }

    [Fact]
    public void AssignVehicle_ShouldMoveOrderToVehicleAssigned()
    {
        var order = CreateOrder();
        var vehicleId = Guid.NewGuid();

        order.AssignVehicle(vehicleId);

        Assert.Equal(vehicleId, order.AssignedVehicleId);
        Assert.Equal(ShipmentOrderStatus.VehicleAssigned, order.Status);
    }

    [Fact]
    public void AssignDriverAndVehicle_ShouldMoveOrderToReadyToStart()
    {
        var order = CreateOrder();

        order.AssignDriver(Guid.NewGuid());
        order.AssignVehicle(Guid.NewGuid());

        Assert.Equal(ShipmentOrderStatus.ReadyToStart, order.Status);
    }

    [Fact]
    public void Start_ShouldWorkOnlyFromReadyToStart()
    {
        var order = CreateOrder();

        Assert.Throws<DomainException>(() => order.Start(DomainTestData.ChangedAt));

        order.AssignDriver(Guid.NewGuid());
        order.AssignVehicle(Guid.NewGuid());
        order.Start(DomainTestData.ChangedAt);

        Assert.Equal(ShipmentOrderStatus.InProgress, order.Status);
        Assert.Equal(DomainTestData.ChangedAt, order.StartedAt);
    }

    [Fact]
    public void MarkDelivered_ShouldWorkOnlyFromInProgress()
    {
        var order = CreateReadyToStartOrder();

        Assert.Throws<DomainException>(() => order.MarkDelivered(DomainTestData.ChangedAt));

        order.Start(DomainTestData.ChangedAt);
        order.MarkDelivered(DomainTestData.ChangedAt);

        Assert.Equal(ShipmentOrderStatus.Delivered, order.Status);
        Assert.Equal(DomainTestData.ChangedAt, order.DeliveredAt);
    }

    [Fact]
    public void Complete_ShouldWorkOnlyFromDelivered()
    {
        var order = CreateReadyToStartOrder();

        Assert.Throws<DomainException>(() => order.Complete(DomainTestData.ChangedAt));

        order.Start(DomainTestData.ChangedAt);
        order.MarkDelivered(DomainTestData.ChangedAt);
        order.Complete(DomainTestData.ChangedAt);

        Assert.Equal(ShipmentOrderStatus.Completed, order.Status);
        Assert.Equal(DomainTestData.ChangedAt, order.CompletedAt);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenOrderIsCompleted()
    {
        var order = CreateReadyToStartOrder();
        order.Start(DomainTestData.ChangedAt);
        order.MarkDelivered(DomainTestData.ChangedAt);
        order.Complete(DomainTestData.ChangedAt);

        Assert.Throws<DomainException>(() => order.Cancel(null, DomainTestData.ChangedAt));
    }

    private static ShipmentOrder CreateReadyToStartOrder()
    {
        var order = CreateOrder();
        order.AssignDriver(Guid.NewGuid());
        order.AssignVehicle(Guid.NewGuid());

        return order;
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
}
