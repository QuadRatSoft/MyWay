using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.Shipments;

public sealed class ShipmentOffer
{
    private ShipmentOffer(
        Guid id,
        Guid shipmentRequestId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        Guid createdByUserId,
        Money offeredPrice,
        string? comment,
        ShipmentOfferStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        ShipmentRequestId = shipmentRequestId;
        CarrierUserId = carrierUserId;
        CarrierCompanyId = carrierCompanyId;
        CreatedByUserId = createdByUserId;
        OfferedPrice = offeredPrice;
        Comment = comment;
        Status = status;
        CreatedAt = createdAt;
    }

    private ShipmentOffer()
    {
        OfferedPrice = null!;
    }

    public Guid Id { get; private set; }

    public Guid ShipmentRequestId { get; private set; }

    public Guid? CarrierUserId { get; private set; }

    public Guid? CarrierCompanyId { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public Money OfferedPrice { get; private set; }

    public string? Comment { get; private set; }

    public ShipmentOfferStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? AcceptedAt { get; private set; }

    public DateTimeOffset? RejectedAt { get; private set; }

    public DateTimeOffset? WithdrawnAt { get; private set; }

    public static ShipmentOffer CreateForUserCarrier(
        Guid shipmentRequestId,
        Guid carrierUserId,
        Guid createdByUserId,
        Money offeredPrice,
        DateTimeOffset createdAt,
        string? comment = null)
    {
        return Create(shipmentRequestId, carrierUserId, null, createdByUserId, offeredPrice, createdAt, comment);
    }

    public static ShipmentOffer CreateForCompanyCarrier(
        Guid shipmentRequestId,
        Guid carrierCompanyId,
        Guid createdByUserId,
        Money offeredPrice,
        DateTimeOffset createdAt,
        string? comment = null)
    {
        return Create(shipmentRequestId, null, carrierCompanyId, createdByUserId, offeredPrice, createdAt, comment);
    }

    public static ShipmentOffer Create(
        Guid shipmentRequestId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        Guid createdByUserId,
        Money offeredPrice,
        DateTimeOffset createdAt,
        string? comment = null)
    {
        ValidateShipmentRequestId(shipmentRequestId);
        ValidateCarrier(carrierUserId, carrierCompanyId);
        ValidateCreatedByUserId(createdByUserId);
        ValidateOfferedPrice(offeredPrice);

        return new ShipmentOffer(
            Guid.NewGuid(),
            shipmentRequestId,
            carrierUserId,
            carrierCompanyId,
            createdByUserId,
            offeredPrice,
            NormalizeOptional(comment),
            ShipmentOfferStatus.Submitted,
            createdAt);
    }

    public void Accept(DateTimeOffset acceptedAt)
    {
        EnsureSubmitted("accept");

        Status = ShipmentOfferStatus.Accepted;
        AcceptedAt = acceptedAt;
    }

    public void Reject(DateTimeOffset rejectedAt)
    {
        EnsureSubmitted("reject");

        Status = ShipmentOfferStatus.Rejected;
        RejectedAt = rejectedAt;
    }

    public void Withdraw(DateTimeOffset withdrawnAt)
    {
        EnsureSubmitted("withdraw");

        Status = ShipmentOfferStatus.Withdrawn;
        WithdrawnAt = withdrawnAt;
    }

    private void EnsureSubmitted(string action)
    {
        if (Status != ShipmentOfferStatus.Submitted)
        {
            throw new DomainException($"Only submitted shipment offers can be {action}ed.");
        }
    }

    private static void ValidateShipmentRequestId(Guid shipmentRequestId)
    {
        if (shipmentRequestId == Guid.Empty)
        {
            throw new DomainException("Shipment request id is required.");
        }
    }

    private static void ValidateCarrier(Guid? carrierUserId, Guid? carrierCompanyId)
    {
        var hasUserCarrier = carrierUserId.HasValue && carrierUserId.Value != Guid.Empty;
        var hasCompanyCarrier = carrierCompanyId.HasValue && carrierCompanyId.Value != Guid.Empty;

        if (!hasUserCarrier && !hasCompanyCarrier)
        {
            throw new DomainException("Shipment offer carrier is required.");
        }

        if (hasUserCarrier && hasCompanyCarrier)
        {
            throw new DomainException("Shipment offer cannot have both user and company carriers.");
        }
    }

    private static void ValidateCreatedByUserId(Guid createdByUserId)
    {
        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Created by user id is required.");
        }
    }

    private static void ValidateOfferedPrice(Money? offeredPrice)
    {
        if (offeredPrice is null)
        {
            throw new DomainException("Offered price is required.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
