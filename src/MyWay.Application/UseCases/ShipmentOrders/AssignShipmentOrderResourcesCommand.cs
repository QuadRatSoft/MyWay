namespace MyWay.Application.UseCases.ShipmentOrders;

public sealed record AssignShipmentOrderResourcesCommand(
    Guid ShipmentOrderId,
    Guid DriverUserId,
    Guid VehicleId);
