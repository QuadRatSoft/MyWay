namespace MyWay.EF.DependencyInjection;

public static class EfServiceCollectionExtensions
{
    public static TServices AddMyWayEf<TServices>(
        this TServices services,
        object? configuration)
    {
        _ = configuration;

        return services;
    }
}
