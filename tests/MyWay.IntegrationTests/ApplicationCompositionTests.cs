using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyWay.Api.DependencyInjection;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.DependencyInjection;
using MyWay.Application.UseCases.Boards;
using MyWay.Application.UseCases.ShipmentOffers;
using MyWay.Application.UseCases.ShipmentOrders;
using MyWay.Application.UseCases.ShipmentRequests;
using MyWay.EF.DependencyInjection;
using MyWay.Infrastructure.DependencyInjection;

namespace MyWay.IntegrationTests;

public sealed class ApplicationCompositionTests
{
    [Fact]
    public void ShouldResolveApplicationComposition()
    {
        var services = new ServiceCollection();
        var environment = new TestWebHostEnvironment();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:MyWay"] =
                    "Host=localhost;Port=5432;Database=myway;Username=myway;Password=myway_password"
            })
            .Build();

        services.AddSingleton<IWebHostEnvironment>(environment);
        services.AddSingleton<IHostEnvironment>(environment);
        services.AddSingleton(new DiagnosticListener("Microsoft.AspNetCore"));
        services.AddSingleton<DiagnosticSource>(
            provider => provider.GetRequiredService<DiagnosticListener>());
        services.AddLogging();

        services.AddMyWayApi();
        services.AddMyWayApplication();
        services.AddMyWayEf(configuration);
        services.AddMyWayInfrastructure(configuration);

        using var serviceProvider = services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateOnBuild = true,
                ValidateScopes = true
            });
        using var scope = serviceProvider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ICurrentUserContext>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IDateTimeProvider>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IUserAccessService>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IAvailabilityService>());

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<CreatePublicShipmentRequestUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<CreateDirectShipmentRequestUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<PublishShipmentRequestUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<CancelShipmentRequestUseCase>());

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<CreateShipmentOfferUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<AcceptShipmentOfferUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<RejectShipmentOfferUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<WithdrawShipmentOfferUseCase>());

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<SearchShipmentRequestBoardUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<SearchCarrierListingBoardUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<GetShipmentRequestDetailsUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<GetCarrierListingDetailsUseCase>());

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<AssignShipmentOrderResourcesUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<StartShipmentOrderUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<MarkShipmentOrderDeliveredUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<CompleteShipmentOrderUseCase>());
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<CancelShipmentOrderUseCase>());
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "MyWay.IntegrationTests";

        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();

        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;

        public string EnvironmentName { get; set; } = Environments.Development;

        public IFileProvider WebRootFileProvider { get; set; } =
            new NullFileProvider();

        public string WebRootPath { get; set; } = AppContext.BaseDirectory;
    }
}
