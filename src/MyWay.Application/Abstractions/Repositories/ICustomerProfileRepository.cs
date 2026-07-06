using MyWay.Core.Profiles;

namespace MyWay.Application.Abstractions.Repositories;

public interface ICustomerProfileRepository
{
    Task<CustomerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<CustomerProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(CustomerProfile entity, CancellationToken cancellationToken = default);
}
