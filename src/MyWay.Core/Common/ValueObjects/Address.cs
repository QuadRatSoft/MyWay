using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Common.ValueObjects;

public sealed record Address
{
    public Address(
        string country,
        string? region,
        string city,
        string street,
        string house,
        string? apartmentOrOffice = null,
        string? postalCode = null,
        decimal? latitude = null,
        decimal? longitude = null)
    {
        if (string.IsNullOrWhiteSpace(country))
        {
            throw new DomainException("Country is required.");
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new DomainException("City is required.");
        }

        if (string.IsNullOrWhiteSpace(street))
        {
            throw new DomainException("Street is required.");
        }

        if (string.IsNullOrWhiteSpace(house))
        {
            throw new DomainException("House is required.");
        }

        Country = country.Trim();
        Region = NormalizeOptional(region);
        City = city.Trim();
        Street = street.Trim();
        House = house.Trim();
        ApartmentOrOffice = NormalizeOptional(apartmentOrOffice);
        PostalCode = NormalizeOptional(postalCode);
        Latitude = latitude;
        Longitude = longitude;
    }

    public string Country { get; }

    public string? Region { get; }

    public string City { get; }

    public string Street { get; }

    public string House { get; }

    public string? ApartmentOrOffice { get; }

    public string? PostalCode { get; }

    public decimal? Latitude { get; }

    public decimal? Longitude { get; }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
