namespace MyWay.Application.Common.Errors;

public static class ResourceReservationErrors
{
    public static ApplicationError AlreadyExistsForOrder(Guid shipmentOrderId)
    {
        return ApplicationError.Create(
            "ResourceReservations.AlreadyExistsForOrder",
            $"Active resource reservation already exists for shipment order '{shipmentOrderId}'.");
    }

    public static ApplicationError DriverUnavailable(Guid driverUserId)
    {
        return ApplicationError.Create(
            "ResourceReservations.DriverUnavailable",
            $"Driver '{driverUserId}' is unavailable for the requested period.");
    }

    public static ApplicationError VehicleUnavailable(Guid vehicleId)
    {
        return ApplicationError.Create(
            "ResourceReservations.VehicleUnavailable",
            $"Vehicle '{vehicleId}' is unavailable for the requested period.");
    }
}
