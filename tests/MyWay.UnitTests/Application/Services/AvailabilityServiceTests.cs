using MyWay.Application.Services;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Resources;
using MyWay.UnitTests.Common;
using MyWay.UnitTests.TestsSupport.Fakes;

namespace MyWay.UnitTests.Application.Services;

public sealed class AvailabilityServiceTests
{
    [Fact]
    public async Task IsDriverAvailable_ShouldReturnTrue_WhenActiveReservationsDoNotExist()
    {
        var service = new AvailabilityService(new InMemoryResourceReservationRepository());

        var result = await service.IsDriverAvailableAsync(Guid.NewGuid(), CreatePeriod());

        Assert.True(result);
    }

    [Fact]
    public async Task IsDriverAvailable_ShouldReturnFalse_WhenActiveReservationOverlaps()
    {
        var repository = new InMemoryResourceReservationRepository();
        var driverUserId = Guid.NewGuid();
        await repository.AddAsync(CreateDriverReservation(driverUserId, CreatePeriod()));
        var service = new AvailabilityService(repository);

        var result = await service.IsDriverAvailableAsync(driverUserId, CreatePeriod());

        Assert.False(result);
    }

    [Fact]
    public async Task IsDriverAvailable_ShouldReturnTrue_WhenActiveReservationDoesNotOverlap()
    {
        var repository = new InMemoryResourceReservationRepository();
        var driverUserId = Guid.NewGuid();
        await repository.AddAsync(CreateDriverReservation(driverUserId, CreateLaterPeriod()));
        var service = new AvailabilityService(repository);

        var result = await service.IsDriverAvailableAsync(driverUserId, CreatePeriod());

        Assert.True(result);
    }

    [Fact]
    public async Task IsDriverAvailable_ShouldReturnFalse_WhenDriverUserIdIsEmpty()
    {
        var service = new AvailabilityService(new InMemoryResourceReservationRepository());

        var result = await service.IsDriverAvailableAsync(Guid.Empty, CreatePeriod());

        Assert.False(result);
    }

    [Fact]
    public async Task IsVehicleAvailable_ShouldReturnTrue_WhenActiveReservationsDoNotExist()
    {
        var service = new AvailabilityService(new InMemoryResourceReservationRepository());

        var result = await service.IsVehicleAvailableAsync(Guid.NewGuid(), CreatePeriod());

        Assert.True(result);
    }

    [Fact]
    public async Task IsVehicleAvailable_ShouldReturnFalse_WhenActiveReservationOverlaps()
    {
        var repository = new InMemoryResourceReservationRepository();
        var vehicleId = Guid.NewGuid();
        await repository.AddAsync(CreateVehicleReservation(vehicleId, CreatePeriod()));
        var service = new AvailabilityService(repository);

        var result = await service.IsVehicleAvailableAsync(vehicleId, CreatePeriod());

        Assert.False(result);
    }

    [Fact]
    public async Task IsVehicleAvailable_ShouldReturnTrue_WhenActiveReservationDoesNotOverlap()
    {
        var repository = new InMemoryResourceReservationRepository();
        var vehicleId = Guid.NewGuid();
        await repository.AddAsync(CreateVehicleReservation(vehicleId, CreateLaterPeriod()));
        var service = new AvailabilityService(repository);

        var result = await service.IsVehicleAvailableAsync(vehicleId, CreatePeriod());

        Assert.True(result);
    }

    [Fact]
    public async Task IsVehicleAvailable_ShouldReturnFalse_WhenVehicleIdIsEmpty()
    {
        var service = new AvailabilityService(new InMemoryResourceReservationRepository());

        var result = await service.IsVehicleAvailableAsync(Guid.Empty, CreatePeriod());

        Assert.False(result);
    }

    [Fact]
    public async Task HasOverlappingReservation_ShouldReturnFalse_WhenDriverAndVehicleAreMissing()
    {
        var service = new AvailabilityService(new InMemoryResourceReservationRepository());

        var result = await service.HasOverlappingReservationAsync(null, null, CreatePeriod());

        Assert.False(result);
    }

    [Fact]
    public async Task HasOverlappingReservation_ShouldReturnTrue_WhenDriverReservationOverlaps()
    {
        var repository = new InMemoryResourceReservationRepository();
        var driverUserId = Guid.NewGuid();
        await repository.AddAsync(CreateDriverReservation(driverUserId, CreatePeriod()));
        var service = new AvailabilityService(repository);

        var result = await service.HasOverlappingReservationAsync(driverUserId, null, CreatePeriod());

        Assert.True(result);
    }

    [Fact]
    public async Task HasOverlappingReservation_ShouldReturnTrue_WhenVehicleReservationOverlaps()
    {
        var repository = new InMemoryResourceReservationRepository();
        var vehicleId = Guid.NewGuid();
        await repository.AddAsync(CreateVehicleReservation(vehicleId, CreatePeriod()));
        var service = new AvailabilityService(repository);

        var result = await service.HasOverlappingReservationAsync(null, vehicleId, CreatePeriod());

        Assert.True(result);
    }

    [Fact]
    public async Task HasOverlappingReservation_ShouldReturnFalse_WhenReservationsDoNotOverlap()
    {
        var repository = new InMemoryResourceReservationRepository();
        var driverUserId = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        await repository.AddAsync(CreateDriverReservation(driverUserId, CreateLaterPeriod()));
        await repository.AddAsync(CreateVehicleReservation(vehicleId, CreateLaterPeriod()));
        var service = new AvailabilityService(repository);

        var result = await service.HasOverlappingReservationAsync(driverUserId, vehicleId, CreatePeriod());

        Assert.False(result);
    }

    private static ResourceReservation CreateDriverReservation(Guid driverUserId, DateRange period)
    {
        return ResourceReservation.Create(
            Guid.NewGuid(),
            driverUserId,
            null,
            period,
            DomainTestData.CreatedAt);
    }

    private static ResourceReservation CreateVehicleReservation(Guid vehicleId, DateRange period)
    {
        return ResourceReservation.Create(
            Guid.NewGuid(),
            null,
            vehicleId,
            period,
            DomainTestData.CreatedAt);
    }

    private static DateRange CreatePeriod()
    {
        return new DateRange(
            new DateTimeOffset(2026, 7, 7, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 7, 17, 0, 0, TimeSpan.Zero));
    }

    private static DateRange CreateLaterPeriod()
    {
        return new DateRange(
            new DateTimeOffset(2026, 7, 8, 9, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 7, 8, 17, 0, 0, TimeSpan.Zero));
    }
}
