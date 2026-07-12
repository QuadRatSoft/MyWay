using System;
using Microsoft.AspNetCore.Http;
using MyWay.Api.CurrentUser;

namespace MyWay.IntegrationTests;

public sealed class HeaderCurrentUserContextTests
{
    [Fact]
    public void ShouldBeAuthenticated_WhenHeaderContainsValidUserId()
    {
        var expectedUserId = Guid.NewGuid();
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderCurrentUserContext.UserIdHeaderName] =
            expectedUserId.ToString();

        var context = CreateCurrentUserContext(httpContext);

        Assert.True(context.IsAuthenticated);
        Assert.Equal(expectedUserId, context.UserId);
        Assert.Null(context.AuthUserId);
    }

    [Fact]
    public void ShouldBeUnauthenticated_WhenHeaderIsMissing()
    {
        var context = CreateCurrentUserContext(new DefaultHttpContext());

        Assert.False(context.IsAuthenticated);
        Assert.Null(context.UserId);
        Assert.Null(context.AuthUserId);
    }

    [Fact]
    public void ShouldBeUnauthenticated_WhenHeaderIsInvalid()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderCurrentUserContext.UserIdHeaderName] =
            "not-a-guid";

        var context = CreateCurrentUserContext(httpContext);

        Assert.False(context.IsAuthenticated);
        Assert.Null(context.UserId);
        Assert.Null(context.AuthUserId);
    }

    [Fact]
    public void ShouldBeUnauthenticated_WhenHeaderContainsEmptyGuid()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers[HeaderCurrentUserContext.UserIdHeaderName] =
            Guid.Empty.ToString();

        var context = CreateCurrentUserContext(httpContext);

        Assert.False(context.IsAuthenticated);
        Assert.Null(context.UserId);
        Assert.Null(context.AuthUserId);
    }

    private static HeaderCurrentUserContext CreateCurrentUserContext(
        HttpContext httpContext)
    {
        return new HeaderCurrentUserContext(
            new HttpContextAccessor
            {
                HttpContext = httpContext
            });
    }
}
