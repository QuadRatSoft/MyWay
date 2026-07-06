using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Profiles;

public sealed class CustomerProfile
{
    private CustomerProfile(
        Guid id,
        Guid userId,
        string displayName,
        DateTimeOffset createdAt,
        bool isActive)
    {
        Id = id;
        UserId = userId;
        DisplayName = displayName;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string DisplayName { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public static CustomerProfile Create(Guid userId, string displayName)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException("User id is required.");
        }

        ValidateDisplayName(displayName);

        return new CustomerProfile(
            Guid.NewGuid(),
            userId,
            displayName.Trim(),
            DateTimeOffset.UtcNow,
            isActive: true);
    }

    public void UpdateDisplayName(string displayName)
    {
        ValidateDisplayName(displayName);

        DisplayName = displayName.Trim();
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
}
