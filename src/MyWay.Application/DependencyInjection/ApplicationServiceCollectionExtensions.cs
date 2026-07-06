namespace MyWay.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static TServices AddMyWayApplication<TServices>(this TServices services)
        where TServices : notnull
    {
        return services;
    }
}
