using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;
using MyWay.Core.Shipments;

namespace MyWay.Application.UseCases.ShipmentRequests;

public sealed class CreatePublicShipmentRequestUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentRequestRepository shipmentRequestRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public CreatePublicShipmentRequestUseCase(
        ICurrentUserContext currentUserContext,
        IUserAccessService userAccessService,
        IShipmentRequestRepository shipmentRequestRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        this.currentUserContext = currentUserContext;
        this.userAccessService = userAccessService;
        this.shipmentRequestRepository = shipmentRequestRepository;
        this.unitOfWork = unitOfWork;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> ExecuteAsync(
        CreatePublicShipmentRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid currentUserId)
        {
            return Result<Guid>.Failure(CommonErrors.Unauthorized());
        }

        var canActAsCustomer = await userAccessService.CanActAsCustomerAsync(
            currentUserId,
            command.CustomerUserId,
            command.CustomerCompanyId,
            cancellationToken);

        if (!canActAsCustomer)
        {
            return Result<Guid>.Failure(AccessErrors.Forbidden());
        }

        try
        {
            var request = ShipmentRequest.CreatePublic(
                currentUserId,
                command.CustomerUserId,
                command.CustomerCompanyId,
                command.PickupAddress,
                command.DeliveryAddress,
                command.CargoDetails,
                command.CustomerPrice,
                command.PlannedPickupAt,
                command.PlannedDeliveryAt,
                dateTimeProvider.UtcNow);

            await shipmentRequestRepository.AddAsync(request, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(request.Id);
        }
        catch (DomainException exception)
        {
            return Result<Guid>.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }
}
