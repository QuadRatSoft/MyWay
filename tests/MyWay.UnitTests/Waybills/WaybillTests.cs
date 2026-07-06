using MyWay.Core.Common.Exceptions;
using MyWay.Core.Waybills;
using MyWay.UnitTests.Common;

namespace MyWay.UnitTests.Waybills;

public sealed class WaybillTests
{
    [Fact]
    public void Create_ShouldCreateWaybillInDraftStatus()
    {
        var waybill = CreateWaybill();

        Assert.Equal(WaybillStatus.Draft, waybill.Status);
        Assert.NotEqual(Guid.Empty, waybill.Id);
        Assert.Equal(DomainTestData.CreatedAt, waybill.CreatedAt);
    }

    [Fact]
    public void Create_ShouldThrow_WhenShipmentOrderIdIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CreateWaybill(shipmentOrderId: Guid.Empty));

        Assert.Contains("shipment order", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenNumberIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CreateWaybill(number: ""));

        Assert.Contains("number", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCustomerIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Waybill.Create(
                Guid.NewGuid(),
                "WB-001",
                null,
                null,
                Guid.NewGuid(),
                null,
                Guid.NewGuid(),
                Guid.NewGuid(),
                "A123AA",
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreatePeriod(),
                DomainTestData.CreatedAt));

        Assert.Contains("customer", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyCustomersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => Waybill.Create(
                Guid.NewGuid(),
                "WB-001",
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                Guid.NewGuid(),
                Guid.NewGuid(),
                "A123AA",
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreatePeriod(),
                DomainTestData.CreatedAt));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenCarrierIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => Waybill.Create(
                Guid.NewGuid(),
                "WB-001",
                Guid.NewGuid(),
                null,
                null,
                null,
                Guid.NewGuid(),
                Guid.NewGuid(),
                "A123AA",
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreatePeriod(),
                DomainTestData.CreatedAt));

        Assert.Contains("carrier", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyCarriersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => Waybill.Create(
                Guid.NewGuid(),
                "WB-001",
                Guid.NewGuid(),
                null,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "A123AA",
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreatePeriod(),
                DomainTestData.CreatedAt));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenDriverUserIdIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CreateWaybill(driverUserId: Guid.Empty));

        Assert.Contains("driver", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenVehicleIdIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CreateWaybill(vehicleId: Guid.Empty));

        Assert.Contains("vehicle id", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenVehiclePlateNumberIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CreateWaybill(vehiclePlateNumber: ""));

        Assert.Contains("plate", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenOdometerStartIsNegative()
    {
        var exception = Assert.Throws<DomainException>(
            () => CreateWaybill(odometerStartKm: -1));

        Assert.Contains("odometer start", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Close_ShouldThrow_WhenOdometerEndIsLessThanOdometerStart()
    {
        var waybill = CreateWaybill(odometerStartKm: 100);
        waybill.Issue(DomainTestData.ChangedAt);

        var exception = Assert.Throws<DomainException>(
            () => waybill.Close(DomainTestData.ChangedAt, odometerEndKm: 90));

        Assert.Contains("odometer end", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Issue_ShouldMoveDraftWaybillToIssued()
    {
        var waybill = CreateWaybill();

        waybill.Issue(DomainTestData.ChangedAt);

        Assert.Equal(WaybillStatus.Issued, waybill.Status);
        Assert.Equal(DomainTestData.ChangedAt, waybill.IssuedAt);
    }

    [Fact]
    public void Issue_ShouldThrow_WhenWaybillIsAlreadyIssued()
    {
        var waybill = CreateWaybill();
        waybill.Issue(DomainTestData.ChangedAt);

        Assert.Throws<DomainException>(() => waybill.Issue(DomainTestData.ChangedAt));
    }

    [Fact]
    public void Close_ShouldMoveIssuedWaybillToClosed()
    {
        var waybill = CreateWaybill(odometerStartKm: 100);
        waybill.Issue(DomainTestData.ChangedAt);

        waybill.Close(DomainTestData.ChangedAt, odometerEndKm: 120);

        Assert.Equal(WaybillStatus.Closed, waybill.Status);
        Assert.Equal(120m, waybill.OdometerEndKm);
        Assert.Equal(DomainTestData.ChangedAt, waybill.ClosedAt);
    }

    [Fact]
    public void Close_ShouldThrow_WhenWaybillIsDraft()
    {
        var waybill = CreateWaybill();

        Assert.Throws<DomainException>(() => waybill.Close(DomainTestData.ChangedAt));
    }

    [Fact]
    public void Cancel_ShouldWorkForDraftWaybill()
    {
        var waybill = CreateWaybill();

        waybill.Cancel("Changed plans", DomainTestData.ChangedAt);

        Assert.Equal(WaybillStatus.Cancelled, waybill.Status);
        Assert.Equal("Changed plans", waybill.CancellationReason);
    }

    [Fact]
    public void Cancel_ShouldWorkForIssuedWaybill()
    {
        var waybill = CreateWaybill();
        waybill.Issue(DomainTestData.ChangedAt);

        waybill.Cancel(null, DomainTestData.ChangedAt);

        Assert.Equal(WaybillStatus.Cancelled, waybill.Status);
        Assert.Equal(DomainTestData.ChangedAt, waybill.CancelledAt);
    }

    [Fact]
    public void Cancel_ShouldThrow_WhenWaybillIsClosed()
    {
        var waybill = CreateWaybill();
        waybill.Issue(DomainTestData.ChangedAt);
        waybill.Close(DomainTestData.ChangedAt);

        Assert.Throws<DomainException>(() => waybill.Cancel(null, DomainTestData.ChangedAt));
    }

    [Fact]
    public void UpdateInfo_ShouldWorkOnlyForDraftWaybill()
    {
        var waybill = CreateWaybill();

        waybill.UpdateInfo(
            "WB-002",
            "B456BB",
            DomainTestData.CreateAddress("NewPickup"),
            DomainTestData.CreateAddress("NewDelivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreatePeriod(),
            driverName: "Ivan Driver",
            odometerStartKm: 20,
            comment: "Updated");

        Assert.Equal("WB-002", waybill.Number);
        Assert.Equal("B456BB", waybill.VehiclePlateNumber);
        Assert.Equal("Ivan Driver", waybill.DriverName);
        Assert.Equal(20m, waybill.OdometerStartKm);

        waybill.Issue(DomainTestData.ChangedAt);

        Assert.Throws<DomainException>(
            () => waybill.UpdateInfo(
                "WB-003",
                "C789CC",
                DomainTestData.CreateAddress("Pickup"),
                DomainTestData.CreateAddress("Delivery"),
                DomainTestData.CreateCargo(),
                DomainTestData.CreatePeriod()));
    }

    private static Waybill CreateWaybill(
        Guid? shipmentOrderId = null,
        string number = "WB-001",
        Guid? driverUserId = null,
        Guid? vehicleId = null,
        string vehiclePlateNumber = "A123AA",
        decimal? odometerStartKm = 10)
    {
        return Waybill.Create(
            shipmentOrderId ?? Guid.NewGuid(),
            number,
            Guid.NewGuid(),
            null,
            Guid.NewGuid(),
            null,
            driverUserId ?? Guid.NewGuid(),
            vehicleId ?? Guid.NewGuid(),
            vehiclePlateNumber,
            DomainTestData.CreateAddress("Pickup"),
            DomainTestData.CreateAddress("Delivery"),
            DomainTestData.CreateCargo(),
            DomainTestData.CreatePeriod(),
            DomainTestData.CreatedAt,
            driverName: "Ivan Driver",
            driverLicenseNumber: "DL-123",
            vehicleBrand: "GAZ",
            vehicleModel: "Gazelle",
            odometerStartKm: odometerStartKm,
            comment: "Initial waybill");
    }
}
