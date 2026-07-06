using MyWay.Application.Common.Pagination;

namespace MyWay.UnitTests.Application;

public sealed class PaginationRequestTests
{
    [Fact]
    public void Create_ShouldNormalizePageNumberToMinimum()
    {
        var request = new PaginationRequest(0, 10);

        Assert.Equal(1, request.PageNumber);
        Assert.Equal(10, request.PageSize);
    }

    [Fact]
    public void Create_ShouldNormalizePageSizeToMinimum()
    {
        var request = new PaginationRequest(2, 0);

        Assert.Equal(2, request.PageNumber);
        Assert.Equal(1, request.PageSize);
    }

    [Fact]
    public void Create_ShouldNormalizePageSizeToMaximum()
    {
        var request = new PaginationRequest(2, 101);

        Assert.Equal(2, request.PageNumber);
        Assert.Equal(100, request.PageSize);
    }
}
