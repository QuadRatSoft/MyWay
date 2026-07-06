using MyWay.Application.Common.Pagination;

namespace MyWay.UnitTests.Application;

public sealed class PagedResultTests
{
    [Fact]
    public void Create_ShouldCalculateTotalPages()
    {
        var result = PagedResult<int>.Create([1, 2, 3], 1, 10, 25);

        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public void Create_ShouldSetTotalPagesToZero_WhenTotalCountIsZero()
    {
        var result = PagedResult<int>.Create([], 1, 10, 0);

        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public void Create_ShouldCalculateHasPreviousPage()
    {
        var firstPage = PagedResult<int>.Create([1], 1, 10, 25);
        var secondPage = PagedResult<int>.Create([2], 2, 10, 25);

        Assert.False(firstPage.HasPreviousPage);
        Assert.True(secondPage.HasPreviousPage);
    }

    [Fact]
    public void Create_ShouldCalculateHasNextPage()
    {
        var secondPage = PagedResult<int>.Create([2], 2, 10, 25);
        var lastPage = PagedResult<int>.Create([3], 3, 10, 25);

        Assert.True(secondPage.HasNextPage);
        Assert.False(lastPage.HasNextPage);
    }
}
