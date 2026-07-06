using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Reviews;

namespace MyWay.EF.Repositories;

public sealed class EfReviewRepository : IReviewRepository
{
    private readonly MyWayDbContext dbContext;

    public EfReviewRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<Review?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Reviews.FirstOrDefaultAsync(review => review.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Review>> GetByShipmentOrderIdAsync(
        Guid shipmentOrderId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .Where(review => review.ShipmentOrderId == shipmentOrderId)
            .OrderBy(review => review.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Review>> GetByTargetAsync(
        ReviewTargetType targetType,
        Guid targetId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .Where(review => review.TargetType == targetType && review.TargetId == targetId)
            .OrderBy(review => review.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AddAsync(Review entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Reviews.AddAsync(entity, cancellationToken);
    }
}
