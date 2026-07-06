namespace MyWay.Application.Common.Errors;

public sealed record ApplicationError
{
    private ApplicationError(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; }

    public string Message { get; }

    public static ApplicationError Create(string code, string message)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Error code is required.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Error message is required.", nameof(message));
        }

        return new ApplicationError(code.Trim(), message.Trim());
    }
}
