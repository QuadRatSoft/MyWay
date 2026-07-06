namespace MyWay.Core.Shipments;

public enum ShipmentOrderStatus
{
    Created = 1,
    DriverAssigned = 2,
    VehicleAssigned = 3,
    ReadyToStart = 4,
    InProgress = 5,
    Delivered = 6,
    Completed = 7,
    Cancelled = 8
}
