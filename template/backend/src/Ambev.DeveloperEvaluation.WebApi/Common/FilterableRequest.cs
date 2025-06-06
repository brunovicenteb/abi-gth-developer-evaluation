namespace Ambev.DeveloperEvaluation.WebApi.Common;

public abstract class FilterableRequest
{
    public Dictionary<string, string> Filters { get; private set; } = [];
    public string OrderBy { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

    public Dictionary<string, string> PrepareFilters(HttpContext httpContext)
    {
        var filters = httpContext.Request.Query
            .Where(q => !string.Equals(q.Key, nameof(Page).ToLower(), StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(q.Key, nameof(Size).ToLower(), StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(q.Key, nameof(OrderBy).ToLower(), StringComparison.OrdinalIgnoreCase))
            .ToDictionary(q => q.Key, q => q.Value.ToString());
        return Filters = filters; ;
    }
}