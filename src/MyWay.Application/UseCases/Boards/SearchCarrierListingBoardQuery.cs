using MyWay.Application.Common.Pagination;

namespace MyWay.Application.UseCases.Boards;

public sealed record SearchCarrierListingBoardQuery(
    CarrierListingBoardFilter Filter,
    PaginationRequest Pagination);
