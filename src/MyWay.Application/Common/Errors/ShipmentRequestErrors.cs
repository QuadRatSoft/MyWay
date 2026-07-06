namespace MyWay.Application.Common.Errors;

public static class ShipmentRequestErrors
{
    public static ApplicationError NotFound(Guid id)
    {
        return ApplicationError.Create(
            "ShipmentRequests.NotFound",
            $"Shipment request '{id}' was not found.");
    }
}
