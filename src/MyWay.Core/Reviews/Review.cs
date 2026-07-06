using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Reviews;

public sealed class Review
{
    private Review(
        Guid id,
        Guid shipmentOrderId,
        Guid fromUserId,
        ReviewTargetType targetType,
        Guid targetId,
        int rating,
        string? text,
        DateTimeOffset createdAt)
    {
        Id = id;
        ShipmentOrderId = shipmentOrderId;
        FromUserId = fromUserId;
        TargetType = targetType;
        TargetId = targetId;
        Rating = rating;
        Text = text;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid ShipmentOrderId { get; private set; }

    public Guid FromUserId { get; private set; }

    public ReviewTargetType TargetType { get; private set; }

    public Guid TargetId { get; private set; }

    public int Rating { get; private set; }

    public string? Text { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Review CreateForCustomerProfile(
        Guid shipmentOrderId,
        Guid fromUserId,
        Guid customerProfileId,
        int rating,
        DateTimeOffset createdAt,
        string? text = null)
    {
        return Create(
            shipmentOrderId,
            fromUserId,
            ReviewTargetType.CustomerProfile,
            customerProfileId,
            rating,
            createdAt,
            text);
    }

    public static Review CreateForCarrierProfile(
        Guid shipmentOrderId,
        Guid fromUserId,
        Guid carrierProfileId,
        int rating,
        DateTimeOffset createdAt,
        string? text = null)
    {
        return Create(
            shipmentOrderId,
            fromUserId,
            ReviewTargetType.CarrierProfile,
            carrierProfileId,
            rating,
            createdAt,
            text);
    }

    public static Review CreateForDriverProfile(
        Guid shipmentOrderId,
        Guid fromUserId,
        Guid driverProfileId,
        int rating,
        DateTimeOffset createdAt,
        string? text = null)
    {
        return Create(
            shipmentOrderId,
            fromUserId,
            ReviewTargetType.DriverProfile,
            driverProfileId,
            rating,
            createdAt,
            text);
    }

    private static Review Create(
        Guid shipmentOrderId,
        Guid fromUserId,
        ReviewTargetType targetType,
        Guid targetId,
        int rating,
        DateTimeOffset createdAt,
        string? text = null)
    {
        ValidateShipmentOrderId(shipmentOrderId);
        ValidateFromUserId(fromUserId);
        ValidateTargetType(targetType);
        ValidateTargetId(targetId);
        ValidateRating(rating);

        return new Review(
            Guid.NewGuid(),
            shipmentOrderId,
            fromUserId,
            targetType,
            targetId,
            rating,
            NormalizeOptional(text),
            createdAt);
    }

    private static void ValidateShipmentOrderId(Guid shipmentOrderId)
    {
        if (shipmentOrderId == Guid.Empty)
        {
            throw new DomainException("Shipment order id is required.");
        }
    }

    private static void ValidateFromUserId(Guid fromUserId)
    {
        if (fromUserId == Guid.Empty)
        {
            throw new DomainException("From user id is required.");
        }
    }

    private static void ValidateTargetType(ReviewTargetType targetType)
    {
        if (!Enum.IsDefined(targetType) || targetType == 0)
        {
            throw new DomainException("Review target type is required.");
        }
    }

    private static void ValidateTargetId(Guid targetId)
    {
        if (targetId == Guid.Empty)
        {
            throw new DomainException("Review target id is required.");
        }
    }

    private static void ValidateRating(int rating)
    {
        if (rating is < 1 or > 5)
        {
            throw new DomainException("Review rating must be from 1 to 5.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
