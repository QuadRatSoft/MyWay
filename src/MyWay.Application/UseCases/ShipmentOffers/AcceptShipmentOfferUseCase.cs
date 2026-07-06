using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;
using MyWay.Core.Shipments;

namespace MyWay.Application.UseCases.ShipmentOffers;

public sealed class AcceptShipmentOfferUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentRequestRepository shipmentRequestRepository;
    private readonly IShipmentOfferRepository shipmentOfferRepository;
    private readonly IShipmentOrderRepository shipmentOrderRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public AcceptShipmentOfferUseCase(
        ICurrentUserContext currentUserContext,
        IUserAccessService userAccessService,
        IShipmentRequestRepository shipmentRequestRepository,
        IShipmentOfferRepository shipmentOfferRepository,
        IShipmentOrderRepository shipmentOrderRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        this.currentUserContext = currentUserContext;
        this.userAccessService = userAccessService;
        this.shipmentRequestRepository = shipmentRequestRepository;
        this.shipmentOfferRepository = shipmentOfferRepository;
        this.shipmentOrderRepository = shipmentOrderRepository;
        this.unitOfWork = unitOfWork;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> ExecuteAsync(
        AcceptShipmentOfferCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid currentUserId)
        {
            return Result<Guid>.Failure(CommonErrors.Unauthorized());
        }

        var request = await shipmentRequestRepository.GetByIdAsync(
            command.ShipmentRequestId,
            cancellationToken);

        if (request is null)
        {
            return Result<Guid>.Failure(ShipmentRequestErrors.NotFound(command.ShipmentRequestId));
        }

        var offer = await shipmentOfferRepository.GetByIdAsync(
            command.ShipmentOfferId,
            cancellationToken);

        if (offer is null)
        {
            return Result<Guid>.Failure(ShipmentOfferErrors.NotFound(command.ShipmentOfferId));
        }

        if (offer.ShipmentRequestId != request.Id)
        {
            return Result<Guid>.Failure(ShipmentOfferErrors.DoesNotBelongToRequest(offer.Id, request.Id));
        }

        var canActAsCustomer = await userAccessService.CanActAsCustomerAsync(
            currentUserId,
            request.CustomerUserId,
            request.CustomerCompanyId,
            cancellationToken);

        if (!canActAsCustomer)
        {
            return Result<Guid>.Failure(AccessErrors.Forbidden());
        }

        try
        {
            var now = dateTimeProvider.UtcNow;

            offer.Accept(now);
            request.AcceptOffer(offer.Id);

            var order = ShipmentOrder.CreateFromAcceptedOffer(
                request.Id,
                offer.Id,
                currentUserId,
                request.CustomerUserId,
                request.CustomerCompanyId,
                offer.CarrierUserId,
                offer.CarrierCompanyId,
                request.PickupAddress,
                request.DeliveryAddress,
                request.CargoDetails,
                offer.OfferedPrice,
                request.PlannedPickupAt,
                request.PlannedDeliveryAt,
                now);

            await shipmentOrderRepository.AddAsync(order, cancellationToken);

            request.MarkConvertedToOrder();

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(order.Id);
        }
        catch (DomainException exception)
        {
            return Result<Guid>.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }
}
