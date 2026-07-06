using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.Shipments;

public sealed class ShipmentRequest
{
    private ShipmentRequest(
        Guid id,
        Guid createdByUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Guid? targetCarrierListingId,
        ShipmentRequestType type,
        ShipmentRequestStatus status,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        Money customerPrice,
        DateTimeOffset plannedPickupAt,
        DateTimeOffset plannedDeliveryAt,
        DateTimeOffset createdAt)
    {
        Id = id;
        CreatedByUserId = createdByUserId;
        CustomerUserId = customerUserId;
        CustomerCompanyId = customerCompanyId;
        TargetCarrierListingId = targetCarrierListingId;
        Type = type;
        Status = status;
        PickupAddress = pickupAddress;
        DeliveryAddress = deliveryAddress;
        CargoDetails = cargoDetails;
        CustomerPrice = customerPrice;
        PlannedPickupAt = plannedPickupAt;
        PlannedDeliveryAt = plannedDeliveryAt;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public Guid? CustomerUserId { get; private set; }

    public Guid? CustomerCompanyId { get; private set; }

    public Guid? TargetCarrierListingId { get; private set; }

    public ShipmentRequestType Type { get; private set; }

    public ShipmentRequestStatus Status { get; private set; }

    public Address PickupAddress { get; private set; }

    public Address DeliveryAddress { get; private set; }

    public CargoDetails CargoDetails { get; private set; }

    public Money CustomerPrice { get; private set; }

    public DateTimeOffset PlannedPickupAt { get; private set; }

    public DateTimeOffset PlannedDeliveryAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? PublishedAt { get; private set; }

    public DateTimeOffset? CancelledAt { get; private set; }

    public string? CancellationReason { get; private set; }

    public Guid? AcceptedOfferId { get; private set; }

    public static ShipmentRequest CreatePublic(
        Guid createdByUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        Money customerPrice,
        DateTimeOffset plannedPickupAt,
        DateTimeOffset plannedDeliveryAt,
        DateTimeOffset createdAt,
        Guid? targetCarrierListingId = null)
    {
        return Create(
            ShipmentRequestType.Public,
            createdByUserId,
            customerUserId,
            customerCompanyId,
            targetCarrierListingId,
            pickupAddress,
            deliveryAddress,
            cargoDetails,
            customerPrice,
            plannedPickupAt,
            plannedDeliveryAt,
            createdAt);
    }

    public static ShipmentRequest CreateDirectToCarrier(
        Guid createdByUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Guid targetCarrierListingId,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        Money customerPrice,
        DateTimeOffset plannedPickupAt,
        DateTimeOffset plannedDeliveryAt,
        DateTimeOffset createdAt)
    {
        return Create(
            ShipmentRequestType.DirectToCarrier,
            createdByUserId,
            customerUserId,
            customerCompanyId,
            targetCarrierListingId,
            pickupAddress,
            deliveryAddress,
            cargoDetails,
            customerPrice,
            plannedPickupAt,
            plannedDeliveryAt,
            createdAt);
    }

    public void Publish(DateTimeOffset publishedAt)
    {
        if (Status != ShipmentRequestStatus.Draft)
        {
            throw new DomainException("Only draft shipment requests can be published.");
        }

        Status = ShipmentRequestStatus.Published;
        PublishedAt = publishedAt;
    }

    public void Cancel(string? reason, DateTimeOffset cancelledAt)
    {
        if (Status is not ShipmentRequestStatus.Draft and not ShipmentRequestStatus.Published)
        {
            throw new DomainException("Only draft or published shipment requests can be cancelled.");
        }

        Status = ShipmentRequestStatus.Cancelled;
        CancellationReason = NormalizeOptional(reason);
        CancelledAt = cancelledAt;
    }

    public void AcceptOffer(Guid offerId)
    {
        if (Status != ShipmentRequestStatus.Published)
        {
            throw new DomainException("Only published shipment requests can accept offers.");
        }

        if (offerId == Guid.Empty)
        {
            throw new DomainException("Offer id is required.");
        }

        AcceptedOfferId = offerId;
        Status = ShipmentRequestStatus.OfferAccepted;
    }

    public void MarkConvertedToOrder()
    {
        if (Status != ShipmentRequestStatus.OfferAccepted)
        {
            throw new DomainException("Only shipment requests with accepted offers can be converted to orders.");
        }

        Status = ShipmentRequestStatus.ConvertedToOrder;
    }

    private static ShipmentRequest Create(
        ShipmentRequestType type,
        Guid createdByUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        Guid? targetCarrierListingId,
        Address pickupAddress,
        Address deliveryAddress,
        CargoDetails cargoDetails,
        Money customerPrice,
        DateTimeOffset plannedPickupAt,
        DateTimeOffset plannedDeliveryAt,
        DateTimeOffset createdAt)
    {
        ValidateCreatedByUserId(createdByUserId);
        ValidateCustomer(customerUserId, customerCompanyId);
        ValidateTargetCarrierListing(type, targetCarrierListingId);
        ValidateRequiredObjects(pickupAddress, deliveryAddress, cargoDetails, customerPrice);

        return new ShipmentRequest(
            Guid.NewGuid(),
            createdByUserId,
            customerUserId,
            customerCompanyId,
            targetCarrierListingId,
            type,
            ShipmentRequestStatus.Draft,
            pickupAddress,
            deliveryAddress,
            cargoDetails,
            customerPrice,
            plannedPickupAt,
            plannedDeliveryAt,
            createdAt);
    }

    private static void ValidateCreatedByUserId(Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Created by user id is required.");
        }
    }

    private static void ValidateCustomer(Guid? customerUserId, Guid? customerCompanyId)
    {
        var hasUserCustomer = customerUserId.HasValue && customerUserId.Value != Guid.Empty;
        var hasCompanyCustomer = customerCompanyId.HasValue && customerCompanyId.Value != Guid.Empty;

        if (!hasUserCustomer && !hasCompanyCustomer)
        {
            throw new DomainException("Shipment request customer is required.");
        }

        if (hasUserCustomer && hasCompanyCustomer)
        {
            throw new DomainException("Shipment request cannot have both user and company customers.");
        }
    }

    private static void ValidateTargetCarrierListing(
        ShipmentRequestType type,
        Guid? targetCarrierListingId)
    {
        if (!Enum.IsDefined(type) || type == 0)
        {
            throw new DomainException("Shipment request type is required.");
        }

        var hasTarget = targetCarrierListingId.HasValue && targetCarrierListingId.Value != Guid.Empty;

        if (type == ShipmentRequestType.Public && hasTarget)
        {
            throw new DomainException("Public shipment request cannot target a carrier listing.");
        }

        if (type == ShipmentRequestType.DirectToCarrier && !hasTarget)
        {
            throw new DomainException("Direct shipment request requires a target carrier listing.");
        }
    }

    private static void ValidateRequiredObjects(
        Address? pickupAddress,
        Address? deliveryAddress,
        CargoDetails? cargoDetails,
        Money? customerPrice)
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

        if (customerPrice is null)
        {
            throw new DomainException("Customer price is required.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
