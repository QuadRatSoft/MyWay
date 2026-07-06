using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.Resources;

public sealed class ResourceReservation
{
    private ResourceReservation(
        Guid id,
        Guid shipmentOrderId,
        Guid? driverUserId,
        Guid? vehicleId,
        DateRange period,
        ResourceReservationStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        ShipmentOrderId = shipmentOrderId;
        DriverUserId = driverUserId;
        VehicleId = vehicleId;
        Period = period;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid ShipmentOrderId { get; private set; }

    public Guid? DriverUserId { get; private set; }

    public Guid? VehicleId { get; private set; }

    public DateRange Period { get; private set; }

    public ResourceReservationStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public static ResourceReservation Create(
        Guid shipmentOrderId,
        Guid? driverUserId,
        Guid? vehicleId,
        DateRange period,
        DateTimeOffset createdAt)
    {
        ValidateShipmentOrderId(shipmentOrderId);
        ValidateResources(driverUserId, vehicleId);
        ValidatePeriod(period);

        return new ResourceReservation(
            Guid.NewGuid(),
            shipmentOrderId,
            driverUserId,
            vehicleId,
            period,
            ResourceReservationStatus.Active,
            createdAt);
    }

    public void Complete(DateTimeOffset completedAt)
    {
        EnsureActive("completed");

        Status = ResourceReservationStatus.Completed;
        CompletedAt = completedAt;
    }

    public void Cancel(DateTimeOffset cancelledAt)
    {
        EnsureActive("cancelled");

        Status = ResourceReservationStatus.Cancelled;
        CancelledAt = cancelledAt;
    }

    public bool Overlaps(DateRange period)
    {
        if (period is null)
        {
            throw new DomainException("Reservation period is required.");
        }

        return Period.Overlaps(period);
    }

    public bool UsesDriver(Guid driverUserId)
    {
        return driverUserId != Guid.Empty && DriverUserId == driverUserId;
    }

    public bool UsesVehicle(Guid vehicleId)
    {
        return vehicleId != Guid.Empty && VehicleId == vehicleId;
    }

    private void EnsureActive(string action)
    {
        if (Status != ResourceReservationStatus.Active)
        {
            throw new DomainException($"Only active resource reservations can be {action}.");
        }
    }

    private static void ValidateShipmentOrderId(Guid shipmentOrderId)
    {
        if (shipmentOrderId == Guid.Empty)
        {
            throw new DomainException("Shipment order id is required.");
        }
    }

    private static void ValidateResources(Guid? driverUserId, Guid? vehicleId)
    {
        var hasDriver = driverUserId.HasValue && driverUserId.Value != Guid.Empty;
        var hasVehicle = vehicleId.HasValue && vehicleId.Value != Guid.Empty;

        if (!hasDriver && !hasVehicle)
        {
            throw new DomainException("Resource reservation requires a driver or vehicle.");
        }
    }

    private static void ValidatePeriod(DateRange? period)
    {
        if (period is null)
        {
            throw new DomainException("Reservation period is required.");
        }
    }
}
