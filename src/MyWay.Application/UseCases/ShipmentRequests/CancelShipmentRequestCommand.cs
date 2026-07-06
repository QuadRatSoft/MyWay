namespace MyWay.Application.UseCases.ShipmentRequests;

public sealed record CancelShipmentRequestCommand(Guid ShipmentRequestId, string? Reason);
