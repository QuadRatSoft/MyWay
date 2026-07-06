using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.Shipments;

public sealed class ShipmentOrder
{
    private ShipmentOrder(
        Guid id,
        Guid shipmentRequestId,
        Guid acceptedOfferId,
        Guid createdByUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        Money finalPrice,
        decimal? platformCommissionPercent,
        Money? platformCommissionAmount,
        DateTimeOffset plannedPickupAt,
        DateTimeOffset plannedDeliveryAt,
        DateTimeOffset createdAt)
    {
        Id = id;
        ShipmentRequestId = shipmentRequestId;
        AcceptedOfferId = acceptedOfferId;
        CreatedByUserId = createdByUserId;
        CustomerUserId = customerUserId;
        CustomerCompanyId = customerCompanyId;
        CarrierUserId = carrierUserId;
        CarrierCompanyId = carrierCompanyId;
        PickupAddress = pickupAddress;
        DeliveryAddress = deliveryAddress;
        CargoDetails = cargoDetails;
        FinalPrice = finalPrice;
        PlatformCommissionPercent = platformCommissionPercent;
        PlatformCommissionAmount = platformCommissionAmount;
        PlannedPickupAt = plannedPickupAt;
        PlannedDeliveryAt = plannedDeliveryAt;
        CreatedAt = createdAt;
        Status = ShipmentOrderStatus.Created;
    }

    private ShipmentOrder()
    {
        PickupAddress = null!;
        DeliveryAddress = null!;
        CargoDetails = null!;
        FinalPrice = null!;
    }

    public Guid Id { get; private set; }

    public Guid ShipmentRequestId { get; private set; }

    public Guid AcceptedOfferId { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public Guid? CustomerUserId { get; private set; }

    public Guid? CustomerCompanyId { get; private set; }

    public Guid? CarrierUserId { get; private set; }

    public Guid? CarrierCompanyId { get; private set; }

    public Guid? AssignedDriverUserId { get; private set; }

    public Guid? AssignedVehicleId { get; private set; }

    public Address PickupAddress { get; private set; }

    public Address DeliveryAddress { get; private set; }

    public CargoDetails CargoDetails { get; private set; }

    public Money FinalPrice { get; private set; }

    public decimal? PlatformCommissionPercent { get; private set; }

    public Money? PlatformCommissionAmount { get; private set; }

    public ShipmentOrderStatus Status { get; private set; }

    public DateTimeOffset PlannedPickupAt { get; private set; }

    public DateTimeOffset PlannedDeliveryAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? StartedAt { get; private set; }

    public DateTimeOffset? DeliveredAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public static ShipmentOrder CreateFromAcceptedOffer(
        Guid shipmentRequestId,
        Guid acceptedOfferId,
        Guid createdByUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        Money finalPrice,
        DateTimeOffset plannedPickupAt,
        DateTimeOffset plannedDeliveryAt,
        DateTimeOffset createdAt,
        decimal? platformCommissionPercent = null,
        Money? platformCommissionAmount = null)
    {
        ValidateRequiredIds(shipmentRequestId, acceptedOfferId, createdByUserId);
        ValidateParty(customerUserId, customerCompanyId, "customer");
        ValidateParty(carrierUserId, carrierCompanyId, "carrier");
        ValidateRequiredObjects(pickupAddress, deliveryAddress, cargoDetails, finalPrice);

        return new ShipmentOrder(
            Guid.NewGuid(),
            shipmentRequestId,
            acceptedOfferId,
            createdByUserId,
            customerUserId,
            customerCompanyId,
            carrierUserId,
            carrierCompanyId,
            pickupAddress,
            deliveryAddress,
            cargoDetails,
            finalPrice,
            platformCommissionPercent,
            platformCommissionAmount,
            plannedPickupAt,
            plannedDeliveryAt,
            createdAt);
    }

    public void AssignDriver(Guid driverUserId)
    {
        EnsureCanAssignResources();

        if (driverUserId == Guid.Empty)
        {
            throw new DomainException("Driver user id is required.");
        }

        AssignedDriverUserId = driverUserId;
        UpdateAssignmentStatus();
    }

    public void AssignVehicle(Guid vehicleId)
    {
        EnsureCanAssignResources();

        if (vehicleId == Guid.Empty)
        {
            throw new DomainException("Vehicle id is required.");
        }

        AssignedVehicleId = vehicleId;
        UpdateAssignmentStatus();
    }

    public void Start(DateTimeOffset startedAt)
    {
        if (Status != ShipmentOrderStatus.ReadyToStart)
        {
            throw new DomainException("Only ready to start shipment orders can be started.");
        }

        Status = ShipmentOrderStatus.InProgress;
        StartedAt = startedAt;
    }

    public void MarkDelivered(DateTimeOffset deliveredAt)
    {
        if (Status != ShipmentOrderStatus.InProgress)
        {
            throw new DomainException("Only in progress shipment orders can be marked as delivered.");
        }

        Status = ShipmentOrderStatus.Delivered;
        DeliveredAt = deliveredAt;
    }

    public void Complete(DateTimeOffset completedAt)
    {
        if (Status != ShipmentOrderStatus.Delivered)
        {
            throw new DomainException("Only delivered shipment orders can be completed.");
        }

        Status = ShipmentOrderStatus.Completed;
        CompletedAt = completedAt;
    }

    public void Cancel(string? reason, DateTimeOffset cancelledAt)
    {
        if (Status is ShipmentOrderStatus.Completed or ShipmentOrderStatus.Cancelled)
        {
            throw new DomainException("Completed or cancelled shipment orders cannot be cancelled.");
        }

        Status = ShipmentOrderStatus.Cancelled;
        CancellationReason = NormalizeOptional(reason);
        CancelledAt = cancelledAt;
    }

    private void EnsureCanAssignResources()
    {
        if (Status is ShipmentOrderStatus.Completed or ShipmentOrderStatus.Cancelled)
        {
            throw new DomainException("Cannot assign resources to completed or cancelled shipment orders.");
        }
    }

    private void UpdateAssignmentStatus()
    {
        if (Status is not ShipmentOrderStatus.Created
            and not ShipmentOrderStatus.DriverAssigned
            and not ShipmentOrderStatus.VehicleAssigned
            and not ShipmentOrderStatus.ReadyToStart)
        {
            return;
        }

        if (AssignedDriverUserId.HasValue && AssignedVehicleId.HasValue)
        {
            Status = ShipmentOrderStatus.ReadyToStart;
            return;
        }

        if (AssignedDriverUserId.HasValue)
        {
            Status = ShipmentOrderStatus.DriverAssigned;
            return;
        }

        if (AssignedVehicleId.HasValue)
        {
            Status = ShipmentOrderStatus.VehicleAssigned;
        }
    }

    private static void ValidateRequiredIds(
        Guid shipmentRequestId,
        Guid acceptedOfferId,
        Guid createdByUserId)
    {
        if (shipmentRequestId == Guid.Empty)
        {
            throw new DomainException("Shipment request id is required.");
        }

        if (acceptedOfferId == Guid.Empty)
        {
            throw new DomainException("Accepted offer id is required.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Created by user id is required.");
        }
    }

    private static void ValidateParty(Guid? userId, Guid? companyId, string partyName)
    {
        var hasUser = userId.HasValue && userId.Value != Guid.Empty;
        var hasCompany = companyId.HasValue && companyId.Value != Guid.Empty;

        if (!hasUser && !hasCompany)
        {
            throw new DomainException($"Shipment order {partyName} is required.");
        }

        if (hasUser && hasCompany)
        {
            throw new DomainException($"Shipment order cannot have both user and company {partyName}.");
        }
    }

    private static void ValidateRequiredObjects(
        Address? pickupAddress,
        Address? deliveryAddress,
        CargoDetails? cargoDetails,
        Money? finalPrice)
    {
        if (pickupAddress is null)
        {
            throw new DomainException("Pickup address is required.");
        }

        if (deliveryAddress is null)
        {
            throw new DomainException("Delivery address is required.");
        }

        if (cargoDetails is null)
        {
            throw new DomainException("Cargo details are required.");
        }

        if (finalPrice is null)
        {
            throw new DomainException("Final price is required.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
