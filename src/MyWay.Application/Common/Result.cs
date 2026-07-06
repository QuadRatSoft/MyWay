using MyWay.Application.Common.Errors;

namespace MyWay.Application.Common;

public sealed class Result
{
    private Result(bool isSuccess, ApplicationError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ApplicationError? Error { get; }

    public static Result Success()
    {
        return new Result(isSuccess: true, error: null);
    }

    public static Result Failure(ApplicationError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result(isSuccess: false, error);
    }
}
