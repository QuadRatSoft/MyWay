using MyWay.Core.Resources;

namespace MyWay.Application.Abstractions.Repositories;

public interface IResourceReservationRepository
{
    Task<ResourceReservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ResourceReservation>> GetActiveByDriverUserIdAsync(
        Guid driverUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ResourceReservation>> GetActiveByVehicleIdAsync(
        Guid vehicleId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ResourceReservation>> GetActiveByShipmentOrderIdAsync(
        Guid shipmentOrderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(ResourceReservation entity, CancellationToken cancellationToken = default);
}
