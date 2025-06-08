using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Queries.GetSale.TestData;

public static class GetSaleTestData
{
    private static readonly Faker Faker = new("pt_BR");

    private static readonly Faker<SaleItem> _itemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.Id, f => f.Random.Guid())
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 1000));

    private static readonly Faker<Sale> _saleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
        .RuleFor(s => s.Branch, f => f.Address.City())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CreatedAt, f => f.Date.Past())
        .RuleFor(s => s.Items, f => _itemFaker.Generate(f.Random.Number(1, 3)));

    public static GetSaleQuery GenerateValidCommand()
    {
        return new GetSaleQuery(Faker.Random.Guid());
    }

    public static GetSaleQuery GenerateInvalidCommand()
    {
        return new GetSaleQuery(Guid.Empty);
    }

    public static Sale GenerateValidDomainEntity(GetSaleQuery command)
    {
        var sale = _saleFaker.Generate();
        sale.Id = command.Id;
        sale.CalculateTotal();
        return sale;
    }

    public static GetSaleResult GenerateResult(Sale sale)
    {
        return new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };
    }
}
