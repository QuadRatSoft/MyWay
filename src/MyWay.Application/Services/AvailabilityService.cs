using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Resources;

namespace MyWay.Application.Services;

public sealed class AvailabilityService : IAvailabilityService
{
    private readonly IResourceReservationRepository resourceReservationRepository;

    public AvailabilityService(IResourceReservationRepository resourceReservationRepository)
    {
        this.resourceReservationRepository = resourceReservationRepository;
    }

    public async Task<bool> IsDriverAvailableAsync(
        Guid driverUserId,
        DateRange period,
        CancellationToken cancellationToken = default)
    {
        if (driverUserId == Guid.Empty || period is null)
        {
            return false;
        }

        var reservations = await resourceReservationRepository.GetActiveByDriverUserIdAsync(
            driverUserId,
            cancellationToken);

        return !HasOverlappingReservation(reservations, period);
    }

    public async Task<bool> IsVehicleAvailableAsync(
        Guid vehicleId,
        DateRange period,
        CancellationToken cancellationToken = default)
    {
        if (vehicleId == Guid.Empty || period is null)
        {
            return false;
        }

        var reservations = await resourceReservationRepository.GetActiveByVehicleIdAsync(
            vehicleId,
            cancellationToken);

        return !HasOverlappingReservation(reservations, period);
    }

    public async Task<bool> HasOverlappingReservationAsync(
        Guid? driverUserId,
        Guid? vehicleId,
        DateRange period,
        CancellationToken cancellationToken = default)
    {
        if (period is null)
        {
            return false;
        }

        var hasDriver = driverUserId.HasValue && driverUserId.Value != Guid.Empty;
        var hasVehicle = vehicleId.HasValue && vehicleId.Value != Guid.Empty;

        if (!hasDriver && !hasVehicle)
        {
            return false;
        }

        if (hasDriver)
        {
            var driverReservations = await resourceReservationRepository.GetActiveByDriverUserIdAsync(
                driverUserId!.Value,
                cancellationToken);

            if (HasOverlappingReservation(driverReservations, period))
            {
                return true;
            }
        }

        if (hasVehicle)
        {
            var vehicleReservations = await resourceReservationRepository.GetActiveByVehicleIdAsync(
                vehicleId!.Value,
                cancellationToken);

            if (HasOverlappingReservation(vehicleReservations, period))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasOverlappingReservation(
        IEnumerable<ResourceReservation> reservations,
        DateRange period)
    {
        return reservations.Any(reservation => reservation.Overlaps(period));
    }
}
