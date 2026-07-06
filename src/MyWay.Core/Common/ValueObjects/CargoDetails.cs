using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Common.ValueObjects;

public sealed record CargoDetails
{
    public CargoDetails(
        string name,
        decimal weightKg,
        string? description = null,
        decimal? volumeM3 = null,
        decimal? lengthCm = null,
        decimal? widthCm = null,
        decimal? heightCm = null,
        bool isFragile = false,
        bool requiresRefrigeration = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Cargo name is required.");
        }

        if (weightKg < 0)
        {
            throw new DomainException("Cargo weight cannot be negative.");
        }

        if (volumeM3 < 0)
        {
            throw new DomainException("Cargo volume cannot be negative.");
        }

        if (lengthCm < 0)
        {
            throw new DomainException("Cargo length cannot be negative.");
        }

        if (widthCm < 0)
        {
            throw new DomainException("Cargo width cannot be negative.");
        }

        if (heightCm < 0)
        {
            throw new DomainException("Cargo height cannot be negative.");
        }

        Name = name.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        WeightKg = weightKg;
        VolumeM3 = volumeM3;
        LengthCm = lengthCm;
        WidthCm = widthCm;
        HeightCm = heightCm;
        IsFragile = isFragile;
        RequiresRefrigeration = requiresRefrigeration;
    }

    public string Name { get; }

    public string? Description { get; }

    public decimal WeightKg { get; }

    public decimal? VolumeM3 { get; }

    public decimal? LengthCm { get; }

    public decimal? WidthCm { get; }

    public decimal? HeightCm { get; }

    public bool IsFragile { get; }

    public bool RequiresRefrigeration { get; }
}
