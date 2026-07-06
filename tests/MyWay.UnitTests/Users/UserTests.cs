using MyWay.Core.Users;

namespace MyWay.UnitTests.Users;

public sealed class UserTests
{
    private static readonly DateTimeOffset CreatedAt = new(2026, 7, 6, 10, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_ShouldCreateActiveUser()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "User", CreatedAt);

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.True(user.IsActive);
        Assert.Equal(CreatedAt, user.CreatedAt);
    }

    [Fact]
    public void Deactivate_ShouldMakeUserInactive()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "User", CreatedAt);

        user.Deactivate();

        Assert.False(user.IsActive);
    }

    [Fact]
    public void Activate_ShouldMakeUserActiveAgain()
    {
        var user = User.Create(Guid.NewGuid(), "user@example.com", "User", CreatedAt);
        user.Deactivate();

        user.Activate();

        Assert.True(user.IsActive);
    }
}
