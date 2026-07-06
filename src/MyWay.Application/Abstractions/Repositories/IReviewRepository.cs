using MyWay.Core.Reviews;

namespace MyWay.Application.Abstractions.Repositories;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Review>> GetByShipmentOrderIdAsync(
        Guid shipmentOrderId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Review>> GetByTargetAsync(
        ReviewTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Review entity, CancellationToken cancellationToken = default);
}
