using MyWay.Core.Common.Exceptions;
using MyWay.Core.Profiles;

namespace MyWay.UnitTests.Profiles;

public sealed class CarrierProfileTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 7, 6, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void CreateForUser_ShouldCreateCarrierProfileForUser()
    {
        var userId = Guid.NewGuid();

        var profile = CarrierProfile.CreateForUser(userId, "Private carrier", CreatedAt);

        Assert.Equal(userId, profile.UserId);
        Assert.Null(profile.CompanyId);
        Assert.True(profile.IsActive);
        Assert.Equal(CreatedAt, profile.CreatedAt);
    }

    [Fact]
    public void CreateForCompany_ShouldCreateCarrierProfileForCompany()
    {
        var companyId = Guid.NewGuid();

        var profile = CarrierProfile.CreateForCompany(companyId, "Company carrier", CreatedAt);

        Assert.Null(profile.UserId);
        Assert.Equal(companyId, profile.CompanyId);
        Assert.True(profile.IsActive);
        Assert.Equal(CreatedAt, profile.CreatedAt);
    }

    [Fact]
    public void Create_ShouldThrow_WhenOwnerIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CarrierProfile.Create(null, null, "Carrier", CreatedAt));

        Assert.Contains("owner", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyOwnersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => CarrierProfile.Create(Guid.NewGuid(), Guid.NewGuid(), "Carrier", CreatedAt));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
