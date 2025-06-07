using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Queries.GetSales.TestData;

public static class GetSalesTestData
{
    private static readonly Faker Faker = new("pt_BR");

    private static readonly Faker<Sale> _saleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
        .RuleFor(s => s.Branch, f => f.Address.City())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CreatedAt, f => f.Date.Past())
        .RuleFor(s => s.Items, f => new List<SaleItem>
        {
            new SaleItem
            {
                Id = f.Random.Guid(),
                ProductId = f.Random.Guid(),
                ProductName = f.Commerce.ProductName(),
                Quantity = f.Random.Number(1, 5),
                UnitPrice = f.Random.Decimal(10, 200)
            }
        });

    public static GetSalesQuery GenerateValidCommand()
    {
        return new GetSalesQuery
        {
            Page = 1,
            Size = 10,
            OrderBy = "SaleNumber",
            Filters = new Dictionary<string, string> { { "Branch", "*São" } }
        };
    }

    public static GetSalesQuery GenerateInvalidCommand()
    {
        return new GetSalesQuery
        {
            Page = 0,
            Size = 0,
            OrderBy = ""
        };
    }

    public static List<Sale> GenerateValidSalesList()
    {
        var list = _saleFaker.Generate(5);
        list.ForEach(s => s.CalculateTotal());
        return list;
    }

    public static GetSalesResult GeneratePagedResult(List<Sale> sales)
    {
        return new GetSalesResult
        {
            Total = sales.Count,
            Data = sales.Select(s => new SaleListItemDto
            {
                Id = s.Id,
                SaleNumber = s.SaleNumber,
                TotalAmount = s.TotalAmount
            }).ToList()
        };
    }
}