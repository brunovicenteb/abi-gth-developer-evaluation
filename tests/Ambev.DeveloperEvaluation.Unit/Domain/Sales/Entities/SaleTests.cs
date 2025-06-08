using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain.Sales.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Entities;

/// <summary>
/// Contains comprehensive unit tests for the Sale and SaleItem entities,
/// including valid and invalid scenarios.
/// </summary>
public class SaleTests
{
    [Theory(DisplayName = "SaleItem should apply correct discount based on quantity")]
    [InlineData(1, 100, 0)]
    [InlineData(4, 100, 0.10)]
    [InlineData(10, 100, 0.20)]
    public void Given_Quantity_When_ApplyDiscount_Then_ShouldMatchExpectedDiscount(int quantity, decimal price, decimal expectedDiscount)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItem(quantity, price);

        // Act
        item.CalculateTotal();

        // Assert
        Assert.Equal(expectedDiscount, item.Discount);
    }


    [Fact(DisplayName = "SaleItem should throw exception when quantity exceeds maximum allowed")]
    public void Given_QuantityAboveMax_When_ApplyDiscount_Then_ShouldThrow()
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItem(25, 100);

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => item.CalculateTotal());

        // Assert
        Assert.Equal(Sale.MAX_ITEM_LIMIT_EXCEEDED, ex.Message);
    }


    [Theory(DisplayName = "SaleItem total should reflect discount properly")]
    [InlineData(1, 2, 2)] // 1 * 2 * (1 - 0) No discount
    [InlineData(3, 0.5, 1.5)] // 3 * 0.5 * (1 - 0) No discount
    [InlineData(4, 5, 18)] // 4 * 5 * (1 - 0.1) 10% discount
    [InlineData(5, 3, 13.5)] // 5 * 3 * (1 - 0.1) 10% discount
    [InlineData(9, 6, 48.6)] // 2 * 4 * (1 - 0.1) 10% discount
    [InlineData(10, 50, 400)] // 10 * 50 * (1 - 0.2) 20 % discount
    [InlineData(15, 3, 36)] // 15 * 3 * (1 - 0.2) 20 % discount
    [InlineData(19, 1.5, 22.8)] // 15 * 3 * (1 - 0.2) 20 % discount
    public void Given_DiscountedItem_When_CalculateTotal_Then_ResultShouldBeCorrect(int quantity, decimal unitPrice, decimal total)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItem(quantity, unitPrice);

        // Act
        item.CalculateTotal();

        // Assert
        Assert.Equal(total, item.Total);
    }

    [Fact(DisplayName = "Sale total should be sum of all item totals")]
    public void Given_ValidSale_When_CalculateTotal_Then_TotalAmountShouldMatchSum()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.CalculateTotal();

        // Assert
        var expected = 4 * 100 * 0.9m + 10 * 50 * 0.8m;
        Assert.Equal(expected, sale.TotalAmount);
    }

    [Fact(DisplayName = "Sale with no items should throw exception on total calculation")]
    public void Given_EmptySale_When_CalculateTotal_Then_ShouldThrow()
    {
        // Arrange
        var sale = SaleTestData.GenerateEmptySale();

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => sale.CalculateTotal());

        // Assert
        Assert.Equal(Sale.EMPTY_SALE_ITEMS, ex.Message);
    }

    [Fact(DisplayName = "Sale should be marked as cancelled when Cancel is called")]
    public void Given_ValidSale_When_Cancel_Then_IsCancelledShouldBeTrue()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
    }

    [Fact(DisplayName = "SaleItem with minimum discount boundary should apply 10%")]
    public void Given_SaleItemWithQuantity4_When_ApplyDiscount_Then_ShouldBe10Percent()
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItem(4, 200);

        // Act
        item.CalculateTotal();

        // Assert
        Assert.Equal(0.10m, item.Discount);
    }

    [Fact(DisplayName = "SaleItem with quantity just below tier should not apply discount")]
    public void Given_SaleItemWithQuantity3_When_ApplyDiscount_Then_ShouldBeZero()
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItem(3, 150);

        // Act
        item.CalculateTotal();

        // Assert
        Assert.Equal(0, item.Discount);
    }

    [Fact(DisplayName = "SaleItem should be marked as cancelled and total updated")]
    public void Given_SaleItem_When_CancelItemById_Then_ItemShouldBeCancelledAndTotalUpdated()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var itemToCancel = sale.Items.First();
        var itemTotalBeforeCancel = itemToCancel.Total;
        var totalBeforeCancel = sale.TotalAmount;

        // Act
        sale.CancelItemById(itemToCancel.ProductId);

        // Assert
        Assert.True(itemToCancel.IsCancelled);

        var expectedTotal = sale.Items.Where(o => !o.IsCancelled).Sum(i => i.Total);
        Assert.Equal(expectedTotal, sale.TotalAmount);
        Assert.Equal(totalBeforeCancel - itemTotalBeforeCancel, sale.TotalAmount);
    }

    [Fact(DisplayName = "CancelItemById should throw when item does not exist")]
    public void Given_InvalidItemId_When_CancelItemById_Then_ShouldThrow()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var invalidItemId = Guid.NewGuid();

        // Act
        var ex = Assert.Throws<InvalidOperationException>(() => sale.CancelItemById(invalidItemId));

        // Assert
        Assert.Equal($"Item com ProductID {invalidItemId} não encontrado.", ex.Message);
    }

    [Fact(DisplayName = "CancelItemById should throw if item already cancelled")]
    public void Given_AlreadyCancelledItem_When_CancelItemById_Then_ShouldThrow()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var item = sale.Items.First();

        // Act
        sale.CancelItemById(item.ProductId);
        var ex = Assert.Throws<InvalidOperationException>(() => sale.CancelItemById(item.ProductId));

        // Assert
        Assert.Equal("Este item já foi cancelado.", ex.Message);
    }
}