namespace MyWay.Application.Common.Errors;

public static class ShipmentOfferErrors
{
    public static ApplicationError NotFound(Guid id)
    {
        return ApplicationError.Create(
            "ShipmentOffers.NotFound",
            $"Shipment offer '{id}' was not found.");
    }

    public static ApplicationError DoesNotBelongToRequest(Guid offerId, Guid requestId)
    {
        return ApplicationError.Create(
            "ShipmentOffers.DoesNotBelongToRequest",
            $"Shipment offer '{offerId}' does not belong to shipment request '{requestId}'.");
    }
}
