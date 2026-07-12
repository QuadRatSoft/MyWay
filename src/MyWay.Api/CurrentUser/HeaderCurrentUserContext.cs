using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using MyWay.Application.Abstractions.Services;

namespace MyWay.Api.CurrentUser;

public sealed class HeaderCurrentUserContext : ICurrentUserContext
{
    public const string UserIdHeaderName = "X-User-Id";

    private readonly IHttpContextAccessor httpContextAccessor;

    public HeaderCurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated => UserId.HasValue;

    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?
                .Request.Headers[UserIdHeaderName]
                .FirstOrDefault();

            return Guid.TryParse(value, out var userId) && userId != Guid.Empty
                ? userId
                : null;
        }
    }

    public Guid? AuthUserId => null;
}
