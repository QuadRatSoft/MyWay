using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Shipments;

namespace MyWay.Application.UseCases.Boards;

public sealed class GetShipmentRequestDetailsUseCase
{
    private readonly IShipmentRequestRepository shipmentRequestRepository;

    public GetShipmentRequestDetailsUseCase(IShipmentRequestRepository shipmentRequestRepository)
    {
        this.shipmentRequestRepository = shipmentRequestRepository;
    }

    public async Task<Result<ShipmentRequestDetails>> ExecuteAsync(
        GetShipmentRequestDetailsQuery query,
        CancellationToken cancellationToken = default)
    {
        var request = await shipmentRequestRepository.GetByIdAsync(
            query.ShipmentRequestId,
            cancellationToken);

        if (request is null)
        {
            return Result<ShipmentRequestDetails>.Failure(
                ShipmentRequestErrors.NotFound(query.ShipmentRequestId));
        }

        if (request.Status != ShipmentRequestStatus.Published)
        {
            return Result<ShipmentRequestDetails>.Failure(
                ShipmentRequestErrors.NotPublished(request.Id));
        }

        return Result<ShipmentRequestDetails>.Success(MapToDetails(request));
    }

    private static ShipmentRequestDetails MapToDetails(ShipmentRequest request)
    {
        return new ShipmentRequestDetails(
            request.Id,
            request.CreatedByUserId,
            request.CustomerUserId,
            request.CustomerCompanyId,
            request.Type.ToString(),
            request.Status.ToString(),
            request.TargetCarrierListingId,
            request.PickupAddress.City,
            FormatAddressLine(request.PickupAddress),
            request.DeliveryAddress.City,
            FormatAddressLine(request.DeliveryAddress),
            request.CargoDetails.Name,
            request.CargoDetails.WeightKg,
            request.CargoDetails.VolumeM3 ?? 0m,
            request.CustomerPrice.Amount,
            request.CustomerPrice.Currency,
            request.PlannedPickupAt,
            request.PlannedDeliveryAt,
            request.CreatedAt,
            request.PublishedAt,
            request.CancelledAt,
            request.CancellationReason);
    }

    private static string FormatAddressLine(Address address)
    {
        return string.Join(", ", new[]
        {
            address.Country,
            address.Region,
            address.City,
            address.Street,
            address.House,
            address.ApartmentOrOffice,
            address.PostalCode
        }.Where(part => !string.IsNullOrWhiteSpace(part)));
    }
}
