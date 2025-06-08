using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CancelSale.TestData;

public static class CancelSaleTestData
{
    public static CancelSaleResponse GenerateResult() => new(true);

    public static CancelSaleCommand GenerateValidCommand()
    {
        return new CancelSaleCommand(Guid.NewGuid());
    }

    public static CancelSaleCommand GenerateInvalidCommand()
    {
        return new CancelSaleCommand(Guid.Empty);
    }

    public static Sale GenerateValidDomainEntity(CancelSaleCommand command)
    {
        return new Sale
        {
            Id = command.Id,
            SaleNumber = $"SALE-{Random.Shared.Next(1000, 9999)}",
            CreatedAt = DateTime.UtcNow,
            Branch = "São Paulo",
            CustomerId = Guid.NewGuid(),
            Items = new List<SaleItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ProductId = Guid.NewGuid(),
                    ProductName = "Produto X",
                    Quantity = 2,
                    UnitPrice = 50
                }
            }
        };
    }
}