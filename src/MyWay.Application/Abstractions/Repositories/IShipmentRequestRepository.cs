using MyWay.Core.Shipments;

namespace MyWay.Application.Abstractions.Repositories;

public interface IShipmentRequestRepository
{
    Task<ShipmentRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(ShipmentRequest entity, CancellationToken cancellationToken = default);
}
