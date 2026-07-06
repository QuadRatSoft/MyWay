using MyWay.Core.Warehouses;

namespace MyWay.Application.Abstractions.Repositories;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Warehouse>> GetByOwnerUserIdAsync(
        Guid ownerUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Warehouse>> GetByOwnerCompanyIdAsync(
        Guid ownerCompanyId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Warehouse entity, CancellationToken cancellationToken = default);
}
