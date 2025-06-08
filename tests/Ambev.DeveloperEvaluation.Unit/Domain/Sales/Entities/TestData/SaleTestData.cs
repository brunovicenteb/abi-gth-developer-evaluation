using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Entities.TestData;

/// <summary>
/// Provides test data generators for Sale and SaleItem using the Bogus library.
/// Ensures consistency and reusability across all domain-level unit tests for sales.
/// </summary>
public static class SaleTestData
{
    /// <summary>
    /// Generates a valid Sale with a predefined set of items that exercise discount rules.
    /// </summary>
    /// <returns>A Sale instance with valid business rule coverage.</returns>
    public static Sale GenerateValidSale()
    {
        var faker = new Faker();
        var sale = new Sale
        {
            SaleNumber = faker.Random.AlphaNumeric(10),
            CreatedAt = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Person.FullName,
            Branch = faker.Company.CompanyName(),
            Items =
            [
                GenerateSaleItem(4, 100), // 10% discount
                GenerateSaleItem(10, 50)  // 20% discount
            ]
        };
        sale.CalculateTotal();
        return sale;
    }

    /// <summary>
    /// Generates a Sale with no items, used to test validation constraints.
    /// </summary>
    /// <returns>A Sale instance without items.</returns>
    public static Sale GenerateEmptySale()
    {
        var faker = new Faker();
        return new Sale
        {
            SaleNumber = new Faker().Random.AlphaNumeric(10),
            CreatedAt = DateTime.Now,
            CustomerId = Guid.NewGuid(),
            CustomerName = faker.Person.FullName,
            Branch = faker.Company.CompanyName(),
            Items = []
        };
    }

    /// <summary>
    /// Generates a SaleItem with specified quantity and unit price.
    /// </summary>
    /// <param name="quantity">Quantity of the item.</param>
    /// <param name="unitPrice">Unit price of the item.</param>
    /// <returns>A new SaleItem instance with the given values.</returns>
    public static SaleItem GenerateSaleItem(int quantity, decimal unitPrice)
    {
        return new SaleItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = new Faker().Commerce.ProductName(),
            Quantity = quantity,
            UnitPrice = unitPrice
        };
    }
}
