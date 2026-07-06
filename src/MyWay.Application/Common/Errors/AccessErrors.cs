namespace MyWay.Application.Common.Errors;

public static class AccessErrors
{
    public static ApplicationError Forbidden()
    {
        return ApplicationError.Create("Access.Forbidden", "User is not allowed to perform this action.");
    }
}
