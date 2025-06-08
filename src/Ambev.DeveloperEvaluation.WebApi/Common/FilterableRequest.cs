using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ambev.DeveloperEvaluation.WebApi.Common;

public abstract class FilterableRequest
{
    private const byte MIN_PAGE_NUMBER = 1;
    private const byte DEFAULT_PAGE_SIZE = 10;
    public Dictionary<string, string> Filters { get; private set; } = [];
    public string OrderBy { get; set; }
    public int Page { get; set; } = MIN_PAGE_NUMBER;
    public int Size { get; set; } = DEFAULT_PAGE_SIZE;

    public Dictionary<string, string> PrepareFilters(HttpContext httpContext)
    {
        var query = httpContext.Request.Query;

        if (query.TryGetValue("_page", out var page) && int.TryParse(page, out int pageNumber))
            Page = pageNumber;
        if (query.TryGetValue("_size", out var size) && int.TryParse(size, out int pageSize))
            Size = pageSize;
        if (query.TryGetValue("_order", out var order))
            OrderBy = order.ToString().Trim('\"');

        var excludedKeys = new[] { "_page", "_size", "_order" };
        Filters = query
            .Where(q => !excludedKeys.Contains(q.Key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(q => q.Key, q => q.Value.ToString());

        return Filters;
    }
}