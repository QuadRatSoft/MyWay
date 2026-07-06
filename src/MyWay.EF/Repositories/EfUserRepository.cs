using Microsoft.EntityFrameworkCore;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Core.Users;

namespace MyWay.EF.Repositories;

public sealed class EfUserRepository : IUserRepository
{
    private readonly MyWayDbContext dbContext;

    public EfUserRepository(MyWayDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public Task<User?> GetByAuthUserIdAsync(Guid authUserId, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(user => user.AuthUserId == authUserId, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult<User?>(null);
        }

        var normalizedEmail = email.Trim();

        return dbContext.Users.FirstOrDefaultAsync(
            user => Microsoft.EntityFrameworkCore.EF.Functions.ILike(user.Email, normalizedEmail),
            cancellationToken);
    }

    public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(entity, cancellationToken);
    }
}
