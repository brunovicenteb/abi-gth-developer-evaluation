using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CancelSaleItem.TestData;

public static class CancelSaleItemTestData
{
    private static readonly Faker Faker = new("pt_BR");

    private static readonly Faker<SaleItem> _itemFaker = new Faker<SaleItem>()
        .RuleFor(i => i.Id, f => f.Random.Guid())
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Number(1, 5))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 200));

    private static readonly Faker<Sale> _saleFaker = new Faker<Sale>()
        .RuleFor(s => s.Id, f => f.Random.Guid())
        .RuleFor(s => s.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
        .RuleFor(s => s.Branch, f => f.Address.City())
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CreatedAt, f => f.Date.Past());

    public static CancelSaleItemCommand GenerateValidCommand()
    {
        return new CancelSaleItemCommand(Faker.Random.Guid(), Faker.Random.Guid());
    }

    public static CancelSaleItemCommand GenerateInvalidCommand()
    {
        return new CancelSaleItemCommand(Guid.Empty, Guid.Empty);
    }

    public static Sale GenerateValidDomainEntity(CancelSaleItemCommand command, bool includeItem = true)
    {
        var sale = _saleFaker.Generate();
        sale.Id = command.SaleId;

        var items = _itemFaker.Generate(2);
        if (includeItem)
            items[0].ProductId = command.ProductId;
        items.ForEach(o => o.SaleId = sale.Id);

        sale.Items = items;
        sale.CalculateTotal();
        return sale;
    }

    public static CancelSaleItemResult GenerateResult(Sale sale)
    {
        return new CancelSaleItemResult(true);
    }
}
