using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;
using MyWay.Core.Shipments;

namespace MyWay.Application.UseCases.ShipmentOffers;

public sealed class CreateShipmentOfferUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentRequestRepository shipmentRequestRepository;
    private readonly IShipmentOfferRepository shipmentOfferRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public CreateShipmentOfferUseCase(
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

    public async Task<Result<Guid>> ExecuteAsync(
        CreateShipmentOfferCommand command,
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

        if (request.Status != ShipmentRequestStatus.Published)
        {
            return Result<Guid>.Failure(ShipmentRequestErrors.NotPublished(request.Id));
        }

        var canActAsCarrier = await userAccessService.CanActAsCarrierAsync(
            currentUserId,
            command.CarrierUserId,
            command.CarrierCompanyId,
            cancellationToken);

        if (!canActAsCarrier)
        {
            return Result<Guid>.Failure(AccessErrors.Forbidden());
        }

        try
        {
            var createdAt = dateTimeProvider.UtcNow;
            var offer = CreateOffer(command, currentUserId, createdAt);

            await shipmentOfferRepository.AddAsync(offer, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(offer.Id);
        }
        catch (DomainException exception)
        {
            return Result<Guid>.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }

    private static ShipmentOffer CreateOffer(
        CreateShipmentOfferCommand command,
        Guid currentUserId,
        DateTimeOffset createdAt)
    {
        if (command.CarrierUserId.HasValue && !command.CarrierCompanyId.HasValue)
        {
            return ShipmentOffer.CreateForUserCarrier(
                command.ShipmentRequestId,
                command.CarrierUserId.Value,
                currentUserId,
                command.OfferedPrice,
                createdAt,
                command.Comment);
        }

        if (command.CarrierCompanyId.HasValue && !command.CarrierUserId.HasValue)
        {
            return ShipmentOffer.CreateForCompanyCarrier(
                command.ShipmentRequestId,
                command.CarrierCompanyId.Value,
                currentUserId,
                command.OfferedPrice,
                createdAt,
                command.Comment);
        }

        return ShipmentOffer.Create(
            command.ShipmentRequestId,
            command.CarrierUserId,
            command.CarrierCompanyId,
            currentUserId,
            command.OfferedPrice,
            createdAt,
            command.Comment);
    }
}
