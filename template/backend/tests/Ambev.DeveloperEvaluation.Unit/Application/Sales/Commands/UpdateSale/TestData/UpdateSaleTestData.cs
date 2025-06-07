using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.UpdateSale.TestData;

public static class UpdateSaleTestData
{
    private static readonly Faker<UpdateSaleCommand> _commandFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
        .RuleFor(x => x.Branch, f => f.Address.City())
        .RuleFor(x => x.CustomerId, f => f.Random.Guid())
        .RuleFor(x => x.CustomerName, f => f.Company.CompanyName());

    private static readonly Faker<UpdateSaleItemDto> _itemFaker = new Faker<UpdateSaleItemDto>()
        .RuleFor(x => x.ProductId, f => f.Random.Guid())
        .RuleFor(x => x.ProductName, f => f.Commerce.ProductName())
        .RuleFor(x => x.Quantity, f => f.Random.Number(1, 10))
        .RuleFor(x => x.UnitPrice, f => f.Random.Decimal(10, 1000));

    public static UpdateSaleCommand GenerateValidCommand(int productCount)
    {
        var command = _commandFaker.Generate();
        command.Items = _itemFaker.Generate(productCount);
        return command;
    }

    public static UpdateSaleCommand GenerateInvalidCommandWithoutItems()
    {
        var command = _commandFaker.Generate();
        command.Items = [];
        return command;
    }

    public static UpdateSaleResult GenerateResult(Sale sale)
    {
        return new UpdateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };
    }

    public static Sale GenerateValidDomainEntity(UpdateSaleCommand command)
    {
        var sale = new Sale
        {
            Id = command.Id,
            SaleNumber = command.SaleNumber,
            Branch = command.Branch,
            CustomerId = command.CustomerId,
            CreatedAt = DateTime.UtcNow,
            Items = command.Items.Select(x => new SaleItem
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            }).ToList()
        };

        sale.CalculateTotal();
        return sale;
    }
}