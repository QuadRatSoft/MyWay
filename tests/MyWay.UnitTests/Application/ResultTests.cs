using MyWay.Application.Common;
using MyWay.Application.Common.Errors;

namespace MyWay.UnitTests.Application;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResultWithoutError()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        var error = ApplicationError.Create("Users.NotFound", "User was not found.");

        var result = Result.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Failure_ShouldThrow_WhenErrorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Result.Failure(null!));
    }
}
