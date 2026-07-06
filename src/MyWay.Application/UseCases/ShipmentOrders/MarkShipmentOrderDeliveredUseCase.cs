using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;

namespace MyWay.Application.UseCases.ShipmentOrders;

public sealed class MarkShipmentOrderDeliveredUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentOrderRepository shipmentOrderRepository;
    private readonly IResourceReservationRepository resourceReservationRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public MarkShipmentOrderDeliveredUseCase(
        ICurrentUserContext currentUserContext,
        IUserAccessService userAccessService,
        IShipmentOrderRepository shipmentOrderRepository,
        IResourceReservationRepository resourceReservationRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        this.currentUserContext = currentUserContext;
        this.userAccessService = userAccessService;
        this.shipmentOrderRepository = shipmentOrderRepository;
        this.resourceReservationRepository = resourceReservationRepository;
        this.unitOfWork = unitOfWork;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> ExecuteAsync(
        MarkShipmentOrderDeliveredCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid currentUserId)
        {
            return Result.Failure(CommonErrors.Unauthorized());
        }

        var order = await shipmentOrderRepository.GetByIdAsync(command.ShipmentOrderId, cancellationToken);

        if (order is null)
        {
            return Result.Failure(ShipmentOrderErrors.NotFound(command.ShipmentOrderId));
        }

        var canActAsCarrier = await userAccessService.CanActAsCarrierAsync(
            currentUserId,
            order.CarrierUserId,
            order.CarrierCompanyId,
            cancellationToken);

        if (!canActAsCarrier)
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        try
        {
            var now = dateTimeProvider.UtcNow;

            order.MarkDelivered(now);

            var reservations = await resourceReservationRepository.GetActiveByShipmentOrderIdAsync(
                order.Id,
                cancellationToken);

            foreach (var reservation in reservations)
            {
                reservation.Complete(now);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException exception)
        {
            return Result.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }
}
