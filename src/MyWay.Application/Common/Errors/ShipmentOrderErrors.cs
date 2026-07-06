namespace MyWay.Application.Common.Errors;

public static class ShipmentOrderErrors
{
    public static ApplicationError CouldNotCreate(string message)
    {
        return ApplicationError.Create("ShipmentOrders.CouldNotCreate", message);
    }
}
