using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Common;
using MyWay.Application.Common.Pagination;
using MyWay.Core.CarrierListings;

namespace MyWay.Application.UseCases.Boards;

public sealed class SearchCarrierListingBoardUseCase
{
    private readonly ICarrierListingRepository carrierListingRepository;

    public SearchCarrierListingBoardUseCase(ICarrierListingRepository carrierListingRepository)
    {
        this.carrierListingRepository = carrierListingRepository;
    }

    public async Task<Result<PagedResult<CarrierListingBoardItem>>> ExecuteAsync(
        SearchCarrierListingBoardQuery query,
        CancellationToken cancellationToken = default)
    {
        var pagedListings = await carrierListingRepository.SearchVisibleForBoardAsync(
            query.Filter,
            query.Pagination,
            cancellationToken);

        var items = pagedListings.Items
            .Select(MapToBoardItem)
            .ToArray();

        var result = PagedResult<CarrierListingBoardItem>.Create(
            items,
            pagedListings.PageNumber,
            pagedListings.PageSize,
            pagedListings.TotalCount);

        return Result<PagedResult<CarrierListingBoardItem>>.Success(result);
    }

    private static CarrierListingBoardItem MapToBoardItem(CarrierListing listing)
    {
        return new CarrierListingBoardItem(
            listing.Id,
            listing.CarrierUserId,
            listing.CarrierCompanyId,
            listing.Title,
            listing.Status.ToString(),
            listing.IsVisibleOnCarrierBoard,
            listing.BasePrice?.Amount,
            listing.BasePrice?.Currency,
            listing.CreatedAt,
            listing.PublishedAt);
    }
}
