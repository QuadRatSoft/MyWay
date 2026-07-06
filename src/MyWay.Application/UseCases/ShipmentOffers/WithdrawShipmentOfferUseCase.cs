using MyWay.Application.Abstractions;
using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Application.Common;
using MyWay.Application.Common.Errors;
using MyWay.Core.Common.Exceptions;

namespace MyWay.Application.UseCases.ShipmentOffers;

public sealed class WithdrawShipmentOfferUseCase
{
    private readonly ICurrentUserContext currentUserContext;
    private readonly IUserAccessService userAccessService;
    private readonly IShipmentOfferRepository shipmentOfferRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly IDateTimeProvider dateTimeProvider;

    public WithdrawShipmentOfferUseCase(
        ICurrentUserContext currentUserContext,
        IUserAccessService userAccessService,
        IShipmentOfferRepository shipmentOfferRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider)
    {
        this.currentUserContext = currentUserContext;
        this.userAccessService = userAccessService;
        this.shipmentOfferRepository = shipmentOfferRepository;
        this.unitOfWork = unitOfWork;
        this.dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result> ExecuteAsync(
        WithdrawShipmentOfferCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!currentUserContext.IsAuthenticated || currentUserContext.UserId is not Guid currentUserId)
        {
            return Result.Failure(CommonErrors.Unauthorized());
        }

        var offer = await shipmentOfferRepository.GetByIdAsync(
            command.ShipmentOfferId,
            cancellationToken);

        if (offer is null)
        {
            return Result.Failure(ShipmentOfferErrors.NotFound(command.ShipmentOfferId));
        }

        var canActAsCarrier = await userAccessService.CanActAsCarrierAsync(
            currentUserId,
            offer.CarrierUserId,
            offer.CarrierCompanyId,
            cancellationToken);

        if (!canActAsCarrier)
        {
            return Result.Failure(AccessErrors.Forbidden());
        }

        try
        {
            offer.Withdraw(dateTimeProvider.UtcNow);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (DomainException exception)
        {
            return Result.Failure(CommonErrors.DomainRuleViolation(exception.Message));
        }
    }
}
