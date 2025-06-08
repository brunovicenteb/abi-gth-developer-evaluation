namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales
{
    public class GetSalesResult
    {
        public List<SaleListItemDto> Data { get; set; } = [];
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
    }

    public class SaleListItemDto
    {
        public Guid Id { get; set; }
        public string SaleNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}