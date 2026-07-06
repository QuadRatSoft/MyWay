using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Resources;
using MyWay.UnitTests.Common;

namespace MyWay.UnitTests.Resources;

public sealed class ResourceReservationTests
{
    [Fact]
    public void Create_ShouldCreateActiveReservation()
    {
        var reservation = CreateReservation();

        Assert.Equal(ResourceReservationStatus.Active, reservation.Status);
        Assert.NotEqual(Guid.Empty, reservation.Id);
    }

    [Fact]
    public void Create_ShouldThrow_WhenDriverAndVehicleAreMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => ResourceReservation.Create(
                Guid.NewGuid(),
                null,
                null,
                DomainTestData.CreatePeriod(),
                DomainTestData.CreatedAt));

        Assert.Contains("driver or vehicle", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Overlaps_ShouldDetectPeriodIntersection()
    {
        var reservation = CreateReservation();
        var overlappingPeriod = new DateRange(
            DomainTestData.PlannedPickupAt.AddHours(1),
            DomainTestData.PlannedDeliveryAt.AddHours(1));
        var adjacentPeriod = new DateRange(
            DomainTestData.PlannedDeliveryAt,
            DomainTestData.PlannedDeliveryAt.AddHours(1));

        Assert.True(reservation.Overlaps(overlappingPeriod));
        Assert.False(reservation.Overlaps(adjacentPeriod));
    }

    [Fact]
    public void UsesDriver_ShouldReturnTrueForReservedDriver()
    {
        var driverUserId = Guid.NewGuid();
        var reservation = ResourceReservation.Create(
            Guid.NewGuid(),
            driverUserId,
            null,
            DomainTestData.CreatePeriod(),
            DomainTestData.CreatedAt);

        Assert.True(reservation.UsesDriver(driverUserId));
        Assert.False(reservation.UsesDriver(Guid.NewGuid()));
    }

    [Fact]
    public void UsesVehicle_ShouldReturnTrueForReservedVehicle()
    {
        var vehicleId = Guid.NewGuid();
        var reservation = ResourceReservation.Create(
            Guid.NewGuid(),
            null,
            vehicleId,
            DomainTestData.CreatePeriod(),
            DomainTestData.CreatedAt);

        Assert.True(reservation.UsesVehicle(vehicleId));
        Assert.False(reservation.UsesVehicle(Guid.NewGuid()));
    }

    [Fact]
    public void Complete_ShouldWorkOnlyForActiveReservation()
    {
        var reservation = CreateReservation();

        reservation.Complete(DomainTestData.ChangedAt);

        Assert.Equal(ResourceReservationStatus.Completed, reservation.Status);
        Assert.Throws<DomainException>(() => reservation.Complete(DomainTestData.ChangedAt));
    }

    [Fact]
    public void Cancel_ShouldWorkOnlyForActiveReservation()
    {
        var reservation = CreateReservation();

        reservation.Cancel(DomainTestData.ChangedAt);

        Assert.Equal(ResourceReservationStatus.Cancelled, reservation.Status);
        Assert.Throws<DomainException>(() => reservation.Cancel(DomainTestData.ChangedAt));
    }

    private static ResourceReservation CreateReservation()
    {
        return ResourceReservation.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DomainTestData.CreatePeriod(),
            DomainTestData.CreatedAt);
    }
}
