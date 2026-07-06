using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Profiles;

public sealed class DriverProfile
{
    private DriverProfile(
        Guid id,
        Guid userId,
        string displayName,
        string? licenseNumber,
        DateTimeOffset createdAt,
        bool isActive)
    {
        Id = id;
        UserId = userId;
        DisplayName = displayName;
        LicenseNumber = licenseNumber;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string DisplayName { get; private set; }

    public string? LicenseNumber { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public static DriverProfile Create(
        Guid userId,
        string displayName,
        string? licenseNumber = null)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("User id is required.");
        }

        ValidateDisplayName(displayName);

        return new DriverProfile(
            Guid.NewGuid(),
            userId,
            displayName.Trim(),
            NormalizeOptional(licenseNumber),
            DateTimeOffset.UtcNow,
            isActive: true);
    }

    public void UpdateInfo(string displayName, string? licenseNumber = null)
    {
        ValidateDisplayName(displayName);

        DisplayName = displayName.Trim();
        LicenseNumber = NormalizeOptional(licenseNumber);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
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
