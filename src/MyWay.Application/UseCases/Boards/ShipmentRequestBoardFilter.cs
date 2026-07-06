namespace MyWay.Application.UseCases.Boards;

public sealed record ShipmentRequestBoardFilter(
    string? PickupCity,
    string? DeliveryCity,
    DateTimeOffset? PickupFrom,
    DateTimeOffset? PickupTo,
    decimal? MinPriceAmount,
    decimal? MaxPriceAmount,
    string? Currency);
