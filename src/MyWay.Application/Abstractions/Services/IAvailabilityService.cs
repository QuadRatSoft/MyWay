using MyWay.Core.Common.ValueObjects;

namespace MyWay.Application.Abstractions.Services;

public interface IAvailabilityService
{
    Task<bool> IsDriverAvailableAsync(
        Guid driverUserId,
        DateRange period,
        CancellationToken cancellationToken = default);

    Task<bool> IsVehicleAvailableAsync(
        Guid vehicleId,
        DateRange period,
        CancellationToken cancellationToken = default);

    Task<bool> HasOverlappingReservationAsync(
        Guid? driverUserId,
        Guid? vehicleId,
        DateRange period,
        CancellationToken cancellationToken = default);
}
