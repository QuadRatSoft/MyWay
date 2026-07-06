using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;

namespace MyWay.Core.Companies;

public sealed class Company
{
    private Company(
        Guid id,
        string name,
        string? legalName,
        string? taxNumber,
        Address? address,
        ContactInfo? contactInfo,
        Guid createdByUserId,
        DateTimeOffset createdAt,
        bool isActive)
    {
        Id = id;
        Name = name;
        LegalName = legalName;
        TaxNumber = taxNumber;
        Address = address;
        ContactInfo = contactInfo;
        CreatedByUserId = createdByUserId;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public string? LegalName { get; private set; }

    public string? TaxNumber { get; private set; }

    public Address? Address { get; private set; }

    public ContactInfo? ContactInfo { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public static Company Create(
        string name,
        Guid createdByUserId,
        DateTimeOffset createdAt,
        string? legalName = null,
        string? taxNumber = null,
        Address? address = null,
        ContactInfo? contactInfo = null)
    {
        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Created by user id is required.");
        }

        ValidateName(name);

        return new Company(
            Guid.NewGuid(),
            name.Trim(),
            NormalizeOptional(legalName),
            NormalizeOptional(taxNumber),
            address,
            contactInfo,
            createdByUserId,
            createdAt,
            isActive: true);
    }

    public void UpdateInfo(
        string name,
        string? legalName = null,
        string? taxNumber = null,
        Address? address = null,
        ContactInfo? contactInfo = null)
    {
        ValidateName(name);

        Name = name.Trim();
        LegalName = NormalizeOptional(legalName);
        TaxNumber = NormalizeOptional(taxNumber);
        Address = address;
        ContactInfo = contactInfo;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Company name is required.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
