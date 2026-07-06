using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;

namespace MyWay.Application.UseCases.ShipmentOffers;

public sealed class RejectShipmentOfferUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentRequestRepository shipmentRequestRepository;
    private readonly IShipmentOfferRepository shipmentOfferRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public RejectShipmentOfferUseCase(
        ICurrentUserContext currentUserContext,
        IUserAccessService userAccessService,
        IShipmentRequestRepository shipmentRequestRepository,
        IShipmentOfferRepository shipmentOfferRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        this.currentUserContext = currentUserContext;
        this.userAccessService = userAccessService;
        this.shipmentRequestRepository = shipmentRequestRepository;
        this.shipmentOfferRepository = shipmentOfferRepository;
        this.unitOfWork = unitOfWork;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> ExecuteAsync(
        RejectShipmentOfferCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid currentUserId)
        {
            return Result.Failure(CommonErrors.Unauthorized());
        }

        var request = await shipmentRequestRepository.GetByIdAsync(
            command.ShipmentRequestId,
            cancellationToken);

        if (request is null)
        {
            return Result.Failure(ShipmentRequestErrors.NotFound(command.ShipmentRequestId));
        }

        var offer = await shipmentOfferRepository.GetByIdAsync(
            command.ShipmentOfferId,
            cancellationToken);

        if (offer is null)
        {
            return Result.Failure(ShipmentOfferErrors.NotFound(command.ShipmentOfferId));
        }

        if (offer.ShipmentRequestId != request.Id)
        {
            return Result.Failure(ShipmentOfferErrors.DoesNotBelongToRequest(offer.Id, request.Id));
        }

        var canActAsCustomer = await userAccessService.CanActAsCustomerAsync(
            currentUserId,
            request.CustomerUserId,
            request.CustomerCompanyId,
            cancellationToken);

        if (!canActAsCustomer)
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        try
        {
            offer.Reject(dateTimeProvider.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException exception)
        {
            return Result.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }
}
