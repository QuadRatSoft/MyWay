using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Profiles;

public sealed class CarrierProfile
{
    private CarrierProfile(
        Guid id,
        Guid? userId,
        Guid? companyId,
        string displayName,
        string? description,
        DateTimeOffset createdAt,
        bool isActive)
    {
        Id = id;
        UserId = userId;
        CompanyId = companyId;
        DisplayName = displayName;
        Description = description;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public Guid? UserId { get; private set; }

    public Guid? CompanyId { get; private set; }

    public string DisplayName { get; private set; }

    public string? Description { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public static CarrierProfile CreateForUser(
        Guid userId,
        string displayName,
        DateTimeOffset createdAt,
        string? description = null)
    {
        return Create(userId, null, displayName, createdAt, description);
    }

    public static CarrierProfile CreateForCompany(
        Guid companyId,
        string displayName,
        DateTimeOffset createdAt,
        string? description = null)
    {
        return Create(null, companyId, displayName, createdAt, description);
    }

    public static CarrierProfile Create(
        Guid? userId,
        Guid? companyId,
        string displayName,
        DateTimeOffset createdAt,
        string? description = null)
    {
        ValidateOwner(userId, companyId);
        ValidateDisplayName(displayName);

        return new CarrierProfile(
            Guid.NewGuid(),
            userId,
            companyId,
            displayName.Trim(),
            NormalizeOptional(description),
            createdAt,
            isActive: true);
    }

    public void UpdateInfo(string displayName, string? description = null)
    {
        ValidateDisplayName(displayName);

        DisplayName = displayName.Trim();
        Description = NormalizeOptional(description);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private static void ValidateOwner(Guid? userId, Guid? companyId)
    {
        var hasUserOwner = userId.HasValue && userId.Value != Guid.Empty;
        var hasCompanyOwner = companyId.HasValue && companyId.Value != Guid.Empty;

        if (!hasUserOwner && !hasCompanyOwner)
        {
            throw new DomainException("Carrier profile owner is required.");
        }

        if (hasUserOwner && hasCompanyOwner)
        {
            throw new DomainException("Carrier profile cannot have both user and company owners.");
        }
    }

    private static void ValidateDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Display name is required.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
