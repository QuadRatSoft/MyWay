using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Vehicles;

public sealed class Vehicle
{
    private Vehicle(
        Guid id,
        Guid? ownerUserId,
        Guid? ownerCompanyId,
        VehicleType type,
        string? brand,
        string? model,
        string plateNumber,
        decimal capacityKg,
        decimal? volumeM3,
        DateTimeOffset createdAt,
        bool isActive)
    {
        Id = id;
        OwnerUserId = ownerUserId;
        OwnerCompanyId = ownerCompanyId;
        Type = type;
        Brand = brand;
        Model = model;
        PlateNumber = plateNumber;
        CapacityKg = capacityKg;
        VolumeM3 = volumeM3;
        CreatedAt = createdAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public Guid? OwnerUserId { get; private set; }

    public Guid? OwnerCompanyId { get; private set; }

    public VehicleType Type { get; private set; }

    public string? Brand { get; private set; }

    public string? Model { get; private set; }

    public string PlateNumber { get; private set; }

    public decimal CapacityKg { get; private set; }

    public decimal? VolumeM3 { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public bool IsActive { get; private set; }

    public VehicleOwnershipType OwnershipType =>
        OwnerUserId.HasValue ? VehicleOwnershipType.User : VehicleOwnershipType.Company;

    public static Vehicle CreateForUser(
        Guid ownerUserId,
        VehicleType type,
        string plateNumber,
        decimal capacityKg,
        DateTimeOffset createdAt,
        decimal? volumeM3 = null,
        string? brand = null,
        string? model = null)
    {
        return Create(ownerUserId, null, type, plateNumber, capacityKg, createdAt, volumeM3, brand, model);
    }

    public static Vehicle CreateForCompany(
        Guid ownerCompanyId,
        VehicleType type,
        string plateNumber,
        decimal capacityKg,
        DateTimeOffset createdAt,
        decimal? volumeM3 = null,
        string? brand = null,
        string? model = null)
    {
        return Create(null, ownerCompanyId, type, plateNumber, capacityKg, createdAt, volumeM3, brand, model);
    }

    public static Vehicle Create(
        Guid? ownerUserId,
        Guid? ownerCompanyId,
        VehicleType type,
        string plateNumber,
        decimal capacityKg,
        DateTimeOffset createdAt,
        decimal? volumeM3 = null,
        string? brand = null,
        string? model = null)
    {
        ValidateOwner(ownerUserId, ownerCompanyId);
        ValidateInfo(type, plateNumber, capacityKg, volumeM3);

        return new Vehicle(
            Guid.NewGuid(),
            ownerUserId,
            ownerCompanyId,
            type,
            NormalizeOptional(brand),
            NormalizeOptional(model),
            plateNumber.Trim(),
            capacityKg,
            volumeM3,
            createdAt,
            isActive: true);
    }

    public void UpdateInfo(
        VehicleType type,
        string plateNumber,
        decimal capacityKg,
        decimal? volumeM3 = null,
        string? brand = null,
        string? model = null)
    {
        ValidateInfo(type, plateNumber, capacityKg, volumeM3);

        Type = type;
        PlateNumber = plateNumber.Trim();
        CapacityKg = capacityKg;
        VolumeM3 = volumeM3;
        Brand = NormalizeOptional(brand);
        Model = NormalizeOptional(model);
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
            throw new DomainException("Vehicle owner is required.");
        }

        if (hasUserOwner && hasCompanyOwner)
        {
            throw new DomainException("Vehicle cannot have both user and company owners.");
        }
    }

    private static void ValidateInfo(
        VehicleType type,
        string plateNumber,
        decimal capacityKg,
        decimal? volumeM3)
    {
        if (!Enum.IsDefined(type) || type == 0)
        {
            throw new DomainException("Vehicle type is required.");
        }

        if (string.IsNullOrWhiteSpace(plateNumber))
        {
            throw new DomainException("Plate number is required.");
        }

        if (capacityKg < 0)
        {
            throw new DomainException("Vehicle capacity cannot be negative.");
        }

        if (volumeM3 < 0)
        {
            throw new DomainException("Vehicle volume cannot be negative.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
