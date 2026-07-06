namespace MyWay.Application.UseCases.ShipmentOrders;

public sealed record CancelShipmentOrderCommand(Guid ShipmentOrderId, string? Reason);
