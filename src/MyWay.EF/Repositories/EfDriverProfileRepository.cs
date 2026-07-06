using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Profiles;

namespace MyWay.EF.Repositories;

public sealed class EfDriverProfileRepository : IDriverProfileRepository
{
    private readonly MyWayDbContext dbContext;

    public EfDriverProfileRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<DriverProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.DriverProfiles.FirstOrDefaultAsync(profile => profile.Id == id, cancellationToken);
    }

    public Task<DriverProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return dbContext.DriverProfiles.FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(DriverProfile entity, CancellationToken cancellationToken = default)
    {
        await dbContext.DriverProfiles.AddAsync(entity, cancellationToken);
    }
}
