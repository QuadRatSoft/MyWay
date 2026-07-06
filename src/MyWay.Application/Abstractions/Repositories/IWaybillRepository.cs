using MyWay.Core.Waybills;

namespace MyWay.Application.Abstractions.Repositories;

public interface IWaybillRepository
{
    Task<Waybill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(Waybill entity, CancellationToken cancellationToken = default);
}
