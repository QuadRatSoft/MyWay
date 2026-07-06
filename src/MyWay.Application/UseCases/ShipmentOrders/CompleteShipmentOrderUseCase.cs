using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;

namespace MyWay.Application.UseCases.ShipmentOrders;

public sealed class CompleteShipmentOrderUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentOrderRepository shipmentOrderRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public CompleteShipmentOrderUseCase(
        ICurrentUserContext currentUserContext,
        IUserAccessService userAccessService,
        IShipmentOrderRepository shipmentOrderRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        this.currentUserContext = currentUserContext;
        this.userAccessService = userAccessService;
        this.shipmentOrderRepository = shipmentOrderRepository;
        this.unitOfWork = unitOfWork;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> ExecuteAsync(
        CompleteShipmentOrderCommand command,
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

        var canActAsCustomer = await userAccessService.CanActAsCustomerAsync(
            currentUserId,
            order.CustomerUserId,
            order.CustomerCompanyId,
            cancellationToken);

        if (!canActAsCustomer)
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        try
        {
            order.Complete(dateTimeProvider.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException exception)
        {
            return Result.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }
}
