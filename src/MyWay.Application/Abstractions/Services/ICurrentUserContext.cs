namespace MyWay.Application.Abstractions.Services;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }

    Guid? UserId { get; }

    Guid? AuthUserId { get; }
}
