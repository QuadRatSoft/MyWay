using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyWay.EF;

public sealed class MyWayDbContextFactory : IDesignTimeDbContextFactory<MyWayDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway";

    public MyWayDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyWayDbContext>();
        optionsBuilder.UseNpgsql(DefaultConnectionString);

        return new MyWayDbContext(optionsBuilder.Options);
    }
}
