namespace MyWay.Application.Common.Errors;

public static class CommonErrors
{
    public static ApplicationError Unauthorized()
    {
        return ApplicationError.Create("Common.Unauthorized", "User is not authenticated.");
    }

    public static ApplicationError DomainRuleViolation(string message)
    {
        return ApplicationError.Create("Domain.RuleViolation", message);
    }
}
