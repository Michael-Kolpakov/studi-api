namespace Teachio.DAL.Utils.Helpers;

/// <summary>
/// Represents the <see cref="PaginationResponse{T}"/> type.
/// </summary>
/// <typeparam name="T">The type of t.</typeparam>
public class PaginationResponse<T>
{
    private PaginationResponse(
        IEnumerable<T> items,
        int count,
        int pageNumber,
        int pageSize)
    {
        TotalItems = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = pageSize > 0 ? (int)Math.Ceiling(count / (double)pageSize) : 0;
        Entities = items;
    }

    public int TotalItems { get; private set; }

    public int CurrentPage { get; private set; }

    public int TotalPages { get; private set; }

    public int PageSize { get; private set; }

    public IEnumerable<T> Entities { get; set; }

    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="items">The <paramref name="items"/> argument.</param>
    /// <param name="totalItems">The <paramref name="totalItems"/> argument.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>The result produced by this operation.</returns>
    public static PaginationResponse<T> Create(
        IEnumerable<T> items,
        int totalItems,
        int? pageNumber = null,
        int? pageSize = null)
    {
        if (pageNumber is null && pageSize is null)
        {
            return new PaginationResponse<T>(items, totalItems, 1, totalItems);
        }

        if (pageNumber == 0 || pageSize == 0)
        {
            return new PaginationResponse<T>(items, totalItems, pageNumber ?? 0, pageSize ?? 0);
        }

        var resolvedPageNumber = pageNumber ?? 1;
        var resolvedPageSize = pageSize ?? totalItems;

        return new PaginationResponse<T>(items, totalItems, resolvedPageNumber, resolvedPageSize);
    }
}
