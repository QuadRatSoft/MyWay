using Microsoft.EntityFrameworkCore;
using MyWay.Core.Users;
using MyWay.EF;

namespace MyWay.IntegrationTests;

public sealed class MyWayDbContextTests
{
    [Fact]
    public void Can_build_model_with_npgsql_options()
    {
        var options = new DbContextOptionsBuilder<MyWayDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway")
            .Options;

        using var dbContext = new MyWayDbContext(options);

        Assert.Contains(dbContext.Model.GetEntityTypes(), entityType => entityType.ClrType == typeof(User));
    }
}
