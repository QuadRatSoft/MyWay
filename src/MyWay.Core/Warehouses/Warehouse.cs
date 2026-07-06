using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.Warehouses;

public sealed class Warehouse
{
    private Warehouse(
        Guid id,
        Guid? ownerUserId,
        Guid? ownerCompanyId,
        string name,
        Address address,
        ContactInfo? contactInfo,
        string? workingHours,
        string? driverComment,
        DateTimeOffset createdAt,
        bool isActive)
    {
        Id = id;
        OwnerUserId = ownerUserId;
        OwnerCompanyId = ownerCompanyId;
        Name = name;
        Address = address;
        ContactInfo = contactInfo;
        WorkingHours = workingHours;
        DriverComment = driverComment;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public Guid? OwnerUserId { get; private set; }

    public Guid? OwnerCompanyId { get; private set; }

    public string Name { get; private set; }

    public Address Address { get; private set; }

    public ContactInfo? ContactInfo { get; private set; }

    public string? WorkingHours { get; private set; }

    public string? DriverComment { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public static Warehouse CreateForUser(
        Guid ownerUserId,
        string name,
        Address address,
        ContactInfo? contactInfo = null,
        string? workingHours = null,
        string? driverComment = null)
    {
        return Create(ownerUserId, null, name, address, contactInfo, workingHours, driverComment);
    }

    public static Warehouse CreateForCompany(
        Guid ownerCompanyId,
        string name,
        Address address,
        ContactInfo? contactInfo = null,
        string? workingHours = null,
        string? driverComment = null)
    {
        return Create(null, ownerCompanyId, name, address, contactInfo, workingHours, driverComment);
    }

    public static Warehouse Create(
        Guid? ownerUserId,
        Guid? ownerCompanyId,
        string name,
        Address address,
        ContactInfo? contactInfo = null,
        string? workingHours = null,
        string? driverComment = null)
    {
        ValidateOwner(ownerUserId, ownerCompanyId);
        ValidateName(name);
        ValidateAddress(address);

        return new Warehouse(
            Guid.NewGuid(),
            ownerUserId,
            ownerCompanyId,
            name.Trim(),
            address,
            contactInfo,
            NormalizeOptional(workingHours),
            NormalizeOptional(driverComment),
            DateTimeOffset.UtcNow,
            isActive: true);
    }

    public void UpdateInfo(
        string name,
        Address address,
        ContactInfo? contactInfo = null,
        string? workingHours = null,
        string? driverComment = null)
    {
        ValidateName(name);
        ValidateAddress(address);

        Name = name.Trim();
        Address = address;
        ContactInfo = contactInfo;
        WorkingHours = NormalizeOptional(workingHours);
        DriverComment = NormalizeOptional(driverComment);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private static void ValidateOwner(Guid? ownerUserId, Guid? ownerCompanyId)
    {
        var hasUserOwner = ownerUserId.HasValue && ownerUserId.Value != Guid.Empty;
        var hasCompanyOwner = ownerCompanyId.HasValue && ownerCompanyId.Value != Guid.Empty;

        if (!hasUserOwner && !hasCompanyOwner)
        {
            throw new DomainException("Warehouse owner is required.");
        }

        if (hasUserOwner && hasCompanyOwner)
        {
            throw new DomainException("Warehouse cannot have both user and company owners.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Warehouse name is required.");
        }
    }

    private static void ValidateAddress(Address? address)
    {
        if (address is null)
        {
            throw new DomainException("Warehouse address is required.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
