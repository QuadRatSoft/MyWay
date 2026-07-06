using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Users;

public sealed class User
{
    private User(
        Guid id,
        Guid authUserId,
        string email,
        string? phoneNumber,
        string displayName,
        DateTimeOffset createdAt,
        bool isActive)
    {
        Id = id;
        AuthUserId = authUserId;
        Email = email;
        PhoneNumber = phoneNumber;
        DisplayName = displayName;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public Guid AuthUserId { get; private set; }

    public string Email { get; private set; }

    public string? PhoneNumber { get; private set; }

    public string DisplayName { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public static User Create(
        Guid authUserId,
        string email,
        string displayName,
        string? phoneNumber = null)
    {
        if (authUserId == Guid.Empty)
        {
            throw new DomainException("Auth user id is required.");
        }

        ValidateContactInfo(email, displayName);

        return new User(
            Guid.NewGuid(),
            authUserId,
            email.Trim(),
            NormalizeOptional(phoneNumber),
            displayName.Trim(),
            DateTimeOffset.UtcNow,
            isActive: true);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateContactInfo(string email, string displayName, string? phoneNumber = null)
    {
        ValidateContactInfo(email, displayName);

        Email = email.Trim();
        DisplayName = displayName.Trim();
        PhoneNumber = NormalizeOptional(phoneNumber);
    }

    private static void ValidateContactInfo(string email, string displayName)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required.");
        }

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
