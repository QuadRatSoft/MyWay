using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.CarrierListings;

namespace MyWay.Application.UseCases.Boards;

public sealed class GetCarrierListingDetailsUseCase
{
    private readonly ICarrierListingRepository carrierListingRepository;

    public GetCarrierListingDetailsUseCase(ICarrierListingRepository carrierListingRepository)
    {
        this.carrierListingRepository = carrierListingRepository;
    }

    public async Task<Result<CarrierListingDetails>> ExecuteAsync(
        GetCarrierListingDetailsQuery query,
        CancellationToken cancellationToken = default)
    {
        var listing = await carrierListingRepository.GetByIdAsync(
            query.CarrierListingId,
            cancellationToken);

        if (listing is null)
        {
            return Result<CarrierListingDetails>.Failure(
                CarrierListingErrors.NotFound(query.CarrierListingId));
        }

        if (!listing.IsVisibleOnCarrierBoard)
        {
            return Result<CarrierListingDetails>.Failure(
                CarrierListingErrors.NotAvailable(listing.Id));
        }

        return Result<CarrierListingDetails>.Success(MapToDetails(listing));
    }

    private static CarrierListingDetails MapToDetails(CarrierListing listing)
    {
        return new CarrierListingDetails(
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
