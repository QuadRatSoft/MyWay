namespace MyWay.Application.Common.Pagination;

public sealed record PaginationRequest
{
    public const int MinPageNumber = 1;
    public const int MinPageSize = 1;
    public const int MaxPageSize = 100;

    public PaginationRequest(int pageNumber, int pageSize)
    {
        PageNumber = Math.Max(pageNumber, MinPageNumber);
        PageSize = Math.Clamp(pageSize, MinPageSize, MaxPageSize);
    }

    public int PageNumber { get; }

    public int PageSize { get; }
}
