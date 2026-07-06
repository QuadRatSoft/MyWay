using MyWay.Core.Shipments;

namespace MyWay.Application.Abstractions.Repositories;

public interface IShipmentOfferRepository
{
    Task<ShipmentOffer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ShipmentOffer>> GetByShipmentRequestIdAsync(
        Guid shipmentRequestId,
        CancellationToken cancellationToken = default);

    Task AddAsync(ShipmentOffer entity, CancellationToken cancellationToken = default);
}
