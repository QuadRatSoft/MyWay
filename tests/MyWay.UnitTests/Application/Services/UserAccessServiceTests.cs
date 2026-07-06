using MyWay.Application.Services;
using MyWay.Core.Companies;
using MyWay.UnitTests.Common;
using MyWay.UnitTests.TestsSupport.Fakes;

namespace MyWay.UnitTests.Application.Services;

public sealed class UserAccessServiceTests
{
    [Fact]
    public async Task CanActAsCustomer_ShouldReturnTrue_WhenCustomerUserIsCurrentUser()
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var currentUserId = Guid.NewGuid();

        var result = await service.CanActAsCustomerAsync(currentUserId, currentUserId, null);

        Assert.True(result);
    }

    [Fact]
    public async Task CanActAsCustomer_ShouldReturnFalse_WhenCustomerUserIsDifferent()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCustomerAsync(Guid.NewGuid(), Guid.NewGuid(), null);

        Assert.False(result);
    }

    [Fact]
    public async Task CanActAsCustomer_ShouldReturnFalse_WhenUserAndCompanyAreProvided()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCustomerAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task CanActAsCustomer_ShouldReturnFalse_WhenNoUserOrCompanyProvided()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCustomerAsync(Guid.NewGuid(), null, null);

        Assert.False(result);
    }

    [Theory]
    [InlineData(CompanyMemberRole.Owner)]
    [InlineData(CompanyMemberRole.Admin)]
    [InlineData(CompanyMemberRole.Logist)]
    [InlineData(CompanyMemberRole.Manager)]
    public async Task CanActAsCustomer_ShouldReturnTrue_ForAllowedCompanyRole(CompanyMemberRole role)
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await repository.AddAsync(CreateMember(companyId, userId, role));

        var result = await service.CanActAsCustomerAsync(userId, null, companyId);

        Assert.True(result);
    }

    [Fact]
    public async Task CanActAsCustomer_ShouldReturnFalse_ForCompanyDriverRole()
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await repository.AddAsync(CreateMember(companyId, userId, CompanyMemberRole.Driver));

        var result = await service.CanActAsCustomerAsync(userId, null, companyId);

        Assert.False(result);
    }

    [Fact]
    public async Task CanActAsCustomer_ShouldReturnFalse_WhenActiveMembershipDoesNotExist()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCustomerAsync(Guid.NewGuid(), null, Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task CanActAsCarrier_ShouldReturnTrue_WhenCarrierUserIsCurrentUser()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());
        var currentUserId = Guid.NewGuid();

        var result = await service.CanActAsCarrierAsync(currentUserId, currentUserId, null);

        Assert.True(result);
    }

    [Fact]
    public async Task CanActAsCarrier_ShouldReturnFalse_WhenCarrierUserIsDifferent()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCarrierAsync(Guid.NewGuid(), Guid.NewGuid(), null);

        Assert.False(result);
    }

    [Fact]
    public async Task CanActAsCarrier_ShouldReturnFalse_WhenUserAndCompanyAreProvided()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCarrierAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task CanActAsCarrier_ShouldReturnFalse_WhenNoUserOrCompanyProvided()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCarrierAsync(Guid.NewGuid(), null, null);

        Assert.False(result);
    }

    [Theory]
    [InlineData(CompanyMemberRole.Owner)]
    [InlineData(CompanyMemberRole.Admin)]
    [InlineData(CompanyMemberRole.Logist)]
    [InlineData(CompanyMemberRole.Manager)]
    public async Task CanActAsCarrier_ShouldReturnTrue_ForAllowedCompanyRole(CompanyMemberRole role)
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await repository.AddAsync(CreateMember(companyId, userId, role));

        var result = await service.CanActAsCarrierAsync(userId, null, companyId);

        Assert.True(result);
    }

    [Fact]
    public async Task CanActAsCarrier_ShouldReturnFalse_ForCompanyDriverRole()
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await repository.AddAsync(CreateMember(companyId, userId, CompanyMemberRole.Driver));

        var result = await service.CanActAsCarrierAsync(userId, null, companyId);

        Assert.False(result);
    }

    [Fact]
    public async Task CanActAsCarrier_ShouldReturnFalse_WhenActiveMembershipDoesNotExist()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanActAsCarrierAsync(Guid.NewGuid(), null, Guid.NewGuid());

        Assert.False(result);
    }

    [Theory]
    [InlineData(CompanyMemberRole.Owner)]
    [InlineData(CompanyMemberRole.Admin)]
    public async Task CanManageCompany_ShouldReturnTrue_ForAllowedRole(CompanyMemberRole role)
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await repository.AddAsync(CreateMember(companyId, userId, role));

        var result = await service.CanManageCompanyAsync(userId, companyId);

        Assert.True(result);
    }

    [Theory]
    [InlineData(CompanyMemberRole.Logist)]
    [InlineData(CompanyMemberRole.Manager)]
    [InlineData(CompanyMemberRole.Driver)]
    public async Task CanManageCompany_ShouldReturnFalse_ForNotAllowedRole(CompanyMemberRole role)
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await repository.AddAsync(CreateMember(companyId, userId, role));

        var result = await service.CanManageCompanyAsync(userId, companyId);

        Assert.False(result);
    }

    [Fact]
    public async Task CanManageCompany_ShouldReturnFalse_WhenActiveMembershipDoesNotExist()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.CanManageCompanyAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task IsCompanyMember_ShouldReturnTrue_WhenMembershipIsActive()
    {
        var repository = new InMemoryCompanyMemberRepository();
        var service = new UserAccessService(repository);
        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await repository.AddAsync(CreateMember(companyId, userId, CompanyMemberRole.Driver));

        var result = await service.IsCompanyMemberAsync(userId, companyId);

        Assert.True(result);
    }

    [Fact]
    public async Task IsCompanyMember_ShouldReturnFalse_WhenMembershipDoesNotExist()
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());

        var result = await service.IsCompanyMemberAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public async Task IsCompanyMember_ShouldReturnFalse_WhenUserOrCompanyIdIsEmpty(
        bool emptyUserId,
        bool emptyCompanyId)
    {
        var service = new UserAccessService(new InMemoryCompanyMemberRepository());
        var userId = emptyUserId ? Guid.Empty : Guid.NewGuid();
        var companyId = emptyCompanyId ? Guid.Empty : Guid.NewGuid();

        var result = await service.IsCompanyMemberAsync(userId, companyId);

        Assert.False(result);
    }

    private static CompanyMember CreateMember(
        Guid companyId,
        Guid userId,
        CompanyMemberRole role)
    {
        return CompanyMember.Create(companyId, userId, role, DomainTestData.CreatedAt);
    }
}
