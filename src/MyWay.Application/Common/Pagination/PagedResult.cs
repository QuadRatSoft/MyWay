namespace MyWay.Application.Common.Pagination;

public sealed class PagedResult<T>
{
    private PagedResult(
        IReadOnlyCollection<T> items,
        int pageNumber,
        int pageSize,
        int totalCount,
        int totalPages)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
    }

    public IReadOnlyCollection<T> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static PagedResult<T> Create(
        IReadOnlyCollection<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (totalCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count cannot be negative.");
        }

        var paginationRequest = new PaginationRequest(pageNumber, pageSize);
        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)paginationRequest.PageSize);

        return new PagedResult<T>(
            items,
            paginationRequest.PageNumber,
            paginationRequest.PageSize,
            totalCount,
            totalPages);
    }
}
