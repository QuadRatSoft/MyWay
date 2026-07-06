using MyWay.Core.Common.Exceptions;
using MyWay.Core.Profiles;

namespace MyWay.UnitTests.Profiles;

public sealed class CarrierProfileTests
{
    [Fact]
    public void CreateForUser_ShouldCreateCarrierProfileForUser()
    {
        var userId = Guid.NewGuid();

        var profile = CarrierProfile.CreateForUser(userId, "Private carrier");

        Assert.Equal(userId, profile.UserId);
        Assert.Null(profile.CompanyId);
        Assert.True(profile.IsActive);
    }

    [Fact]
    public void CreateForCompany_ShouldCreateCarrierProfileForCompany()
    {
        var companyId = Guid.NewGuid();

        var profile = CarrierProfile.CreateForCompany(companyId, "Company carrier");

        Assert.Null(profile.UserId);
        Assert.Equal(companyId, profile.CompanyId);
        Assert.True(profile.IsActive);
    }

    [Fact]
    public void Create_ShouldThrow_WhenOwnerIsMissing()
    {
        var exception = Assert.Throws<DomainException>(
            () => CarrierProfile.Create(null, null, "Carrier"));

        Assert.Contains("owner", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_ShouldThrow_WhenUserAndCompanyOwnersAreProvided()
    {
        var exception = Assert.Throws<DomainException>(
            () => CarrierProfile.Create(Guid.NewGuid(), Guid.NewGuid(), "Carrier"));

        Assert.Contains("both", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
