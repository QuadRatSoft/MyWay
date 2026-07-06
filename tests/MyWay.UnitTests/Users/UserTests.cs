using MyWay.Core.Users;

namespace MyWay.UnitTests.Users;

public sealed class UserTests
{
    [Fact]
    public void Create_ShouldCreateActiveUser()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "User");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldMakeUserInactive()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "User");

        user.Deactivate();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void Activate_ShouldMakeUserActiveAgain()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "User");
        user.Deactivate();

        user.Activate();

        Assert.True(user.IsActive);
    }
}
