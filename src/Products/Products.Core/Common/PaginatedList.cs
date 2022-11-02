using Microsoft.EntityFrameworkCore;

namespace IGroceryStore.Products.Core.Common;

public record QueryForPaginatedResult(uint PageNumber = 1, uint PageSize = 10);

public class PaginatedList<T>
{
    public List<T> Items { get; }
    public uint PageNumber { get; }
    public uint TotalPages { get; }
    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, uint pageNumber, uint pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (uint)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, uint pageNumber = 1, uint pageSize = 10)
    {
        var count = await source.CountAsync();
        var items = await source.Skip(((int)pageNumber - 1) * (int)pageSize)
            .Take((int)pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }
}

