using MyWay.Application.Abstractions.Repositories;
using MyWay.Application.Abstractions.Services;
using MyWay.Core.Companies;

namespace MyWay.Application.Services;

public sealed class UserAccessService : IUserAccessService
{
    private readonly ICompanyMemberRepository companyMemberRepository;

    public UserAccessService(ICompanyMemberRepository companyMemberRepository)
    {
        this.companyMemberRepository = companyMemberRepository;
    }

    public Task<bool> CanActAsCustomerAsync(
        Guid currentUserId,
        Guid? customerUserId,
        Guid? customerCompanyId,
        CancellationToken cancellationToken = default)
    {
        return CanActAsPartyAsync(
            currentUserId,
            customerUserId,
            customerCompanyId,
            cancellationToken);
    }

    public Task<bool> CanActAsCarrierAsync(
        Guid currentUserId,
        Guid? carrierUserId,
        Guid? carrierCompanyId,
        CancellationToken cancellationToken = default)
    {
        return CanActAsPartyAsync(
            currentUserId,
            carrierUserId,
            carrierCompanyId,
            cancellationToken);
    }

    public async Task<bool> CanManageCompanyAsync(
        Guid currentUserId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        if (currentUserId == Guid.Empty || companyId == Guid.Empty)
        {
            return false;
        }

        var membership = await companyMemberRepository.GetActiveMembershipAsync(
            companyId,
            currentUserId,
            cancellationToken);

        return membership is not null && CanManageCompany(membership.Role);
    }

    public async Task<bool> IsCompanyMemberAsync(
        Guid currentUserId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        if (currentUserId == Guid.Empty || companyId == Guid.Empty)
        {
            return false;
        }

        var membership = await companyMemberRepository.GetActiveMembershipAsync(
            companyId,
            currentUserId,
            cancellationToken);

        return membership is not null;
    }

    private async Task<bool> CanActAsPartyAsync(
        Guid currentUserId,
        Guid? userId,
        Guid? companyId,
        CancellationToken cancellationToken)
    {
        if (currentUserId == Guid.Empty)
        {
            return false;
        }

        if (userId.HasValue && companyId.HasValue)
        {
            return false;
        }

        if (!userId.HasValue && !companyId.HasValue)
        {
            return false;
        }

        if (userId.HasValue)
        {
            return userId.Value == currentUserId;
        }

        if (companyId is not Guid companyMemberCompanyId || companyMemberCompanyId == Guid.Empty)
        {
            return false;
        }

        var membership = await companyMemberRepository.GetActiveMembershipAsync(
            companyMemberCompanyId,
            currentUserId,
            cancellationToken);

        return membership is not null && CanActOnBehalfOfCompany(membership.Role);
    }

    private static bool CanActOnBehalfOfCompany(CompanyMemberRole role)
    {
        return role is CompanyMemberRole.Owner
            or CompanyMemberRole.Admin
            or CompanyMemberRole.Logist
            or CompanyMemberRole.Manager;
    }

    private static bool CanManageCompany(CompanyMemberRole role)
    {
        return role is CompanyMemberRole.Owner or CompanyMemberRole.Admin;
    }
}
