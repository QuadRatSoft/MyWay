namespace MyWay.Application.Common.Errors;

public static class CarrierListingErrors
{
    public static ApplicationError NotFound(Guid id)
    {
        return ApplicationError.Create(
            "CarrierListings.NotFound",
            $"Carrier listing '{id}' was not found.");
    }

    public static ApplicationError NotAvailable(Guid id)
    {
        return ApplicationError.Create(
            "CarrierListings.NotAvailable",
            $"Carrier listing '{id}' is not available on the carrier board.");
    }
}
