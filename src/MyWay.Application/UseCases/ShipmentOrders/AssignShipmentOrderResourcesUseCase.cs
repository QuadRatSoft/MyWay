using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;
using MyWay.Core.Common.ValueObjects;
using MyWay.Core.Resources;

namespace MyWay.Application.UseCases.ShipmentOrders;

public sealed class AssignShipmentOrderResourcesUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentOrderRepository shipmentOrderRepository;
    private readonly IResourceReservationRepository resourceReservationRepository;
    private readonly IAvailabilityService availabilityService;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public AssignShipmentOrderResourcesUseCase(
        ICurrentUserContext currentUserContext,
        IUserAccessService userAccessService,
        IShipmentOrderRepository shipmentOrderRepository,
        IResourceReservationRepository resourceReservationRepository,
        IAvailabilityService availabilityService,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        this.currentUserContext = currentUserContext;
        this.userAccessService = userAccessService;
        this.shipmentOrderRepository = shipmentOrderRepository;
        this.resourceReservationRepository = resourceReservationRepository;
        this.availabilityService = availabilityService;
        this.unitOfWork = unitOfWork;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> ExecuteAsync(
        AssignShipmentOrderResourcesCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid currentUserId)
        {
            return Result<Guid>.Failure(CommonErrors.Unauthorized());
        }

        if (command.DriverUserId == Guid.Empty)
        {
            return Result<Guid>.Failure(ShipmentOrderErrors.DriverRequired());
        }

        if (command.VehicleId == Guid.Empty)
        {
            return Result<Guid>.Failure(ShipmentOrderErrors.VehicleRequired());
        }

        var order = await shipmentOrderRepository.GetByIdAsync(command.ShipmentOrderId, cancellationToken);

        if (order is null)
        {
            return Result<Guid>.Failure(ShipmentOrderErrors.NotFound(command.ShipmentOrderId));
        }

        var canActAsCarrier = await userAccessService.CanActAsCarrierAsync(
            currentUserId,
            order.CarrierUserId,
            order.CarrierCompanyId,
            cancellationToken);

        if (!canActAsCarrier)
        {
            return Result<Guid>.Failure(AccessErrors.Forbidden());
        }

        var activeOrderReservations = await resourceReservationRepository.GetActiveByShipmentOrderIdAsync(
            order.Id,
            cancellationToken);

        if (activeOrderReservations.Count > 0)
        {
            return Result<Guid>.Failure(ResourceReservationErrors.AlreadyExistsForOrder(order.Id));
        }

        try
        {
            var period = new DateRange(order.PlannedPickupAt, order.PlannedDeliveryAt);

            var isDriverAvailable = await availabilityService.IsDriverAvailableAsync(
                command.DriverUserId,
                period,
                cancellationToken);

            if (!isDriverAvailable)
            {
                return Result<Guid>.Failure(ResourceReservationErrors.DriverUnavailable(command.DriverUserId));
            }

            var isVehicleAvailable = await availabilityService.IsVehicleAvailableAsync(
                command.VehicleId,
                period,
                cancellationToken);

            if (!isVehicleAvailable)
            {
                return Result<Guid>.Failure(ResourceReservationErrors.VehicleUnavailable(command.VehicleId));
            }

            order.AssignDriver(command.DriverUserId);
            order.AssignVehicle(command.VehicleId);

            var reservation = ResourceReservation.Create(
                order.Id,
                command.DriverUserId,
                command.VehicleId,
                period,
                dateTimeProvider.UtcNow);

            await resourceReservationRepository.AddAsync(reservation, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(reservation.Id);
        }
        catch (DomainException exception)
        {
            return Result<Guid>.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }
}
