namespace Ambev.DeveloperEvaluation.Domain.Sales.Entities;

public class SaleItem
{
    public const int DISCOUNT_TIER1_MIN = 4;
    public const int DISCOUNT_TIER2_MIN = 10;
    public const int MAX_ITEM_QUANTITY = 20;
    public const decimal DISCOUNT_TIER1_RATE = 0.10m;
    public const decimal DISCOUNT_TIER2_RATE = 0.20m;

    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }

    private void ApplyDiscount()
    {
        if (Quantity < DISCOUNT_TIER1_MIN)
            Discount = decimal.Zero;
        else if (Quantity < DISCOUNT_TIER2_MIN)
            Discount = DISCOUNT_TIER1_RATE;
        else if (Quantity <= MAX_ITEM_QUANTITY)
            Discount = DISCOUNT_TIER2_RATE;
        else
            throw new InvalidOperationException(Sale.MAX_ITEM_LIMIT_EXCEEDED);
    }

    internal void CalculateTotal()
    {
        ApplyDiscount();
        Total = Quantity * UnitPrice * (1 - Discount);
    }
}