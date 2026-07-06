using MyWay.Application.Abstractions;

namespace MyWay.EF;

public sealed class EfUnitOfWork : IUnitOfWork
{
    private readonly MyWayDbContext dbContext;

    public EfUnitOfWork(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
