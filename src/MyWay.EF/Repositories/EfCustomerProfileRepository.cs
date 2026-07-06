using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Profiles;

namespace MyWay.EF.Repositories;

public sealed class EfCustomerProfileRepository : ICustomerProfileRepository
{
    private readonly MyWayDbContext dbContext;

    public EfCustomerProfileRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<CustomerProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.CustomerProfiles.FirstOrDefaultAsync(profile => profile.Id == id, cancellationToken);
    }

    public Task<CustomerProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.CustomerProfiles.FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(CustomerProfile entity, CancellationToken cancellationToken = default)
    {
        await dbContext.CustomerProfiles.AddAsync(entity, cancellationToken);
    }
}
