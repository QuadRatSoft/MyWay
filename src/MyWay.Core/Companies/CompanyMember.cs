using MyWay.Core.Common.Exceptions;

namespace MyWay.Core.Companies;

public sealed class CompanyMember
{
    private CompanyMember(
        Guid id,
        Guid companyId,
        Guid userId,
        CompanyMemberRole role,
        DateTimeOffset joinedAt,
        bool isActive)
    {
        Id = id;
        CompanyId = companyId;
        UserId = userId;
        Role = role;
        JoinedAt = joinedAt;
        IsActive = isActive;
    }

    public Guid Id { get; private set; }

    public Guid CompanyId { get; private set; }

    public Guid UserId { get; private set; }

    public CompanyMemberRole Role { get; private set; }

    public DateTimeOffset JoinedAt { get; private set; }

    public bool IsActive { get; private set; }

    public static CompanyMember Create(
        Guid companyId,
        Guid userId,
        CompanyMemberRole role,
        DateTimeOffset createdAt)
    {
        ValidateIds(companyId, userId);
        ValidateRole(role);

        return new CompanyMember(
            Guid.NewGuid(),
            companyId,
            userId,
            role,
            createdAt,
            isActive: true);
    }

    public void ChangeRole(CompanyMemberRole role)
    {
        ValidateRole(role);

        Role = role;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    private static void ValidateIds(Guid companyId, Guid userId)
    {
        if (companyId == Guid.Empty)
        {
            throw new DomainException("Company id is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException("User id is required.");
        }
    }

    private static void ValidateRole(CompanyMemberRole role)
    {
        if (!Enum.IsDefined(role) || role == 0)
        {
            throw new DomainException("Company member role is required.");
        }
    }
}
