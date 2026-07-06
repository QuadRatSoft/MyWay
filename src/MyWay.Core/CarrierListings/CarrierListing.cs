using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.CarrierListings;

public sealed class CarrierListing
{
    private CarrierListing(
        Guid id,
        Guid carrierProfileId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        string title,
        string? description,
        Money? basePrice,
        CarrierListingStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        CarrierProfileId = carrierProfileId;
        CarrierUserId = carrierUserId;
        CarrierCompanyId = carrierCompanyId;
        Title = title;
        Description = description;
        BasePrice = basePrice;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid CarrierProfileId { get; private set; }

    public Guid? CarrierUserId { get; private set; }

    public Guid? CarrierCompanyId { get; private set; }

    public string Title { get; private set; }

    public string? Description { get; private set; }

    public Money? BasePrice { get; private set; }

    public CarrierListingStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? PublishedAt { get; private set; }

    public DateTimeOffset? LastStatusChangedAt { get; private set; }

    public bool IsVisibleOnCarrierBoard => Status == CarrierListingStatus.Available;

    public static CarrierListing CreateForUserCarrier(
        Guid carrierProfileId,
        Guid carrierUserId,
        string title,
        DateTimeOffset createdAt,
        string? description = null,
        Money? basePrice = null)
    {
        return Create(carrierProfileId, carrierUserId, null, title, createdAt, description, basePrice);
    }

    public static CarrierListing CreateForCompanyCarrier(
        Guid carrierProfileId,
        Guid carrierCompanyId,
        string title,
        DateTimeOffset createdAt,
        string? description = null,
        Money? basePrice = null)
    {
        return Create(carrierProfileId, null, carrierCompanyId, title, createdAt, description, basePrice);
    }

    public static CarrierListing Create(
        Guid carrierProfileId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        string title,
        DateTimeOffset createdAt,
        string? description = null,
        Money? basePrice = null)
    {
        ValidateCarrierProfileId(carrierProfileId);
        ValidateCarrier(carrierUserId, carrierCompanyId);
        ValidateTitle(title);

        return new CarrierListing(
            Guid.NewGuid(),
            carrierProfileId,
            carrierUserId,
            carrierCompanyId,
            title.Trim(),
            NormalizeOptional(description),
            basePrice,
            CarrierListingStatus.Draft,
            createdAt);
    }

    public void SetAvailable(DateTimeOffset changedAt)
    {
        Status = CarrierListingStatus.Available;
        PublishedAt ??= changedAt;
        LastStatusChangedAt = changedAt;
    }

    public void SetBusy(DateTimeOffset changedAt)
    {
        Status = CarrierListingStatus.Busy;
        LastStatusChangedAt = changedAt;
    }

    public void Deactivate(DateTimeOffset changedAt)
    {
        Status = CarrierListingStatus.Inactive;
        LastStatusChangedAt = changedAt;
    }

    public void UpdateInfo(string title, string? description = null, Money? basePrice = null)
    {
        ValidateTitle(title);

        Title = title.Trim();
        Description = NormalizeOptional(description);
        BasePrice = basePrice;
    }

    private static void ValidateCarrierProfileId(Guid carrierProfileId)
    {
        if (carrierProfileId == Guid.Empty)
        {
            throw new DomainException("Carrier profile id is required.");
        }
    }

    private static void ValidateCarrier(Guid? carrierUserId, Guid? carrierCompanyId)
    {
        var hasUserCarrier = carrierUserId.HasValue && carrierUserId.Value != Guid.Empty;
        var hasCompanyCarrier = carrierCompanyId.HasValue && carrierCompanyId.Value != Guid.Empty;

        if (!hasUserCarrier && !hasCompanyCarrier)
        {
            throw new DomainException("Carrier listing carrier is required.");
        }

        if (hasUserCarrier && hasCompanyCarrier)
        {
            throw new DomainException("Carrier listing cannot have both user and company carriers.");
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Carrier listing title is required.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
