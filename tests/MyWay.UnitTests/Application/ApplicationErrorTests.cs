using MyWay.Application.Common.Errors;

namespace MyWay.UnitTests.Application;

public sealed class ApplicationErrorTests
{
    [Fact]
    public void Create_ShouldThrow_WhenCodeIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => ApplicationError.Create(" ", "Message"));
    }

    [Fact]
    public void Create_ShouldThrow_WhenMessageIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => ApplicationError.Create("Code", " "));
    }

    [Fact]
    public void Create_ShouldCreateError()
    {
        var error = ApplicationError.Create("Users.NotFound", "User was not found.");

        Assert.Equal("Users.NotFound", error.Code);
        Assert.Equal("User was not found.", error.Message);
    }
}
