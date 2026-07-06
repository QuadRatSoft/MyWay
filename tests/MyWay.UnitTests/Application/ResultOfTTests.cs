using MyWay.Application.Common;
using MyWay.Application.Common.Errors;

namespace MyWay.UnitTests.Application;

public sealed class ResultOfTTests
{
    [Fact]
    public void Success_ShouldCreateSuccessfulResultWithValue()
    {
        var result = Result<string>.Success("value");

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal("value", result.Value);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Failure_ShouldCreateFailedResultWithError()
    {
        var error = ApplicationError.Create("Users.NotFound", "User was not found.");

        var result = Result<string>.Failure(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Failure_ShouldThrow_WhenErrorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(null!));
    }
}
