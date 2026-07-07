using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyWay.EF;

public sealed class MyWayDbContextFactory : IDesignTimeDbContextFactory<MyWayDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway_password";

    public MyWayDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("MYWAY_CONNECTION_STRING")
            ?? DefaultConnectionString;

        var optionsBuilder = new DbContextOptionsBuilder<MyWayDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new MyWayDbContext(optionsBuilder.Options);
    }
}
