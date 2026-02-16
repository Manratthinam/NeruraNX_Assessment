namespace neuranx.Application.Common.Models;

public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; }
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedResult(IEnumerable<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static PaginatedResult<T> Create(IEnumerable<T> source, int pageIndex, int pageSize, int count)
    {
        return new PaginatedResult<T>(source, count, pageIndex, pageSize);
    }
}
