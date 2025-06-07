using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CreateSale.TestData;

/// <summary>
/// Provides factory methods to generate valid and invalid test data for creating sales.
/// Uses the Bogus Faker library to ensure realistic randomized values.
/// </summary>
public static class CreateSaleTestData
{
    private static readonly Faker<CreateSaleCommand> _createHandlerFaker = new Faker<CreateSaleCommand>()
        .RuleFor(u => u.SaleNumber, f => $"SALE-{f.Random.Number(1000, 9999)}")
        .RuleFor(u => u.Branch, f => f.Address.City())
        .RuleFor(u => u.CustomerId, f => f.Random.Guid())
        .RuleFor(u => u.CustomerName, f => f.Company.CompanyName());

    private static readonly Faker<CreateSaleItemDto> _createProductFaker = new Faker<CreateSaleItemDto>()
        .RuleFor(u => u.ProductId, f => f.Random.Guid())
        .RuleFor(u => u.ProductName, f => f.Commerce.ProductName())
        .RuleFor(u => u.Quantity, f => f.Random.Number(1, 20))
        .RuleFor(u => u.UnitPrice, f => f.Random.Decimal(0.1m, 1000));

    public static CreateSaleCommand GenerateValidCommand(int productCount)
    {
        var command = _createHandlerFaker.Generate();
        command.Items = _createProductFaker.Generate(productCount);
        return command;
    }

    public static CreateSaleCommand GenerateInvalidCommandWithoutItems()
    {
        var command = _createHandlerFaker.Generate();
        command.Items = [];
        return command;
    }

    public static CreateSaleCommand GenerateInvalidCommandWithExcessQuantity(int productCount)
    {
        var command = _createHandlerFaker.Generate();
        command.Items = _createProductFaker.Generate(productCount);
        command.Items.ForEach(o => o.Quantity = Random.Shared.Next(21, 50));
        return command;
    }

    public static CreateSaleResult GenerateResult(Sale sale)
    {
        return new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            TotalAmount = sale.TotalAmount
        };
    }

    public static Sale GenerateValidDomainEntity(CreateSaleCommand command)
    {
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            CustomerId = command.CustomerId,
            SaleNumber = command.SaleNumber,
            Branch = command.Branch,
            Items = command.Items.Select(o => new SaleItem
            {
                Id = Guid.NewGuid(),
                ProductId = o.ProductId,
                ProductName = o.ProductName,
                Quantity = o.Quantity,
                UnitPrice = o.UnitPrice
            }).ToList()
        };
        sale.CalculateTotal();
        return sale;
    }
}