using MyWay.Application.Common.Pagination;

namespace MyWay.Application.UseCases.Boards;

public sealed record SearchShipmentRequestBoardQuery(
    ShipmentRequestBoardFilter Filter,
    PaginationRequest Pagination);
