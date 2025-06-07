using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales
{
    public class GetSalesQuery : IRequest<GetSalesResult>
    {
        public Dictionary<string, string>? Filters { get; set; }
        public string? OrderBy { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}