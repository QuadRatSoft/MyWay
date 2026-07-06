namespace MyWay.Application.Common.Errors;

public static class ShipmentOrderErrors
{
    public static ApplicationError NotFound(Guid id)
    {
        return ApplicationError.Create(
            "ShipmentOrders.NotFound",
            $"Shipment order '{id}' was not found.");
    }

    public static ApplicationError DriverRequired()
    {
        return ApplicationError.Create(
            "ShipmentOrders.DriverRequired",
            "Driver is required for shipment order resources assignment.");
    }

    public static ApplicationError VehicleRequired()
    {
        return ApplicationError.Create(
            "ShipmentOrders.VehicleRequired",
            "Vehicle is required for shipment order resources assignment.");
    }

    public static ApplicationError CouldNotCreate(string message)
    {
        return ApplicationError.Create("ShipmentOrders.CouldNotCreate", message);
    }
}
