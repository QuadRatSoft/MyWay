using MyWay.Application.Common.Errors;

namespace MyWay.Application.Common;

public sealed class Result<T>
{
    private readonly T? value;

    private Result(bool isSuccess, T? value, ApplicationError? error)
    {
        IsSuccess = isSuccess;
        this.value = value;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot access value for a failed result.");
            }

            return value!;
        }
    }

    public ApplicationError? Error { get; }

    public static Result<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<T>(isSuccess: true, value, error: null);
    }

    public static Result<T> Failure(ApplicationError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<T>(isSuccess: false, value: default, error);
    }
}
