namespace MyWay.Application.Abstractions.Services;

public interface IUserAccessService
{
    Task<bool> CanActAsCustomerAsync(
        Guid currentUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        CancellationToken cancellationToken = default);

    Task<bool> CanActAsCarrierAsync(
        Guid currentUserId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        CancellationToken cancellationToken = default);

    Task<bool> CanManageCompanyAsync(
        Guid currentUserId,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<bool> IsCompanyMemberAsync(
        Guid currentUserId,
        Guid companyId,
        CancellationToken cancellationToken = default);
}
