using MyWay.Core.Shipments;

namespace MyWay.Application.Abstractions.Repositories;

public interface IShipmentOrderRepository
{
    Task<ShipmentOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ShipmentOrder>> GetByCustomerUserIdAsync(
        Guid customerUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ShipmentOrder>> GetByCustomerCompanyIdAsync(
        Guid customerCompanyId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ShipmentOrder>> GetByCarrierUserIdAsync(
        Guid carrierUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ShipmentOrder>> GetByCarrierCompanyIdAsync(
        Guid carrierCompanyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(ShipmentOrder entity, CancellationToken cancellationToken = default);
}
