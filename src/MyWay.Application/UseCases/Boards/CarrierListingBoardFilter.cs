namespace MyWay.Application.UseCases.Boards;

public sealed record CarrierListingBoardFilter(
    string? Region,
    string? RouteFrom,
    string? RouteTo,
    decimal? MinPriceAmount,
    decimal? MaxPriceAmount,
    string? Currency);
