using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Sales.Entities;

public class SaleItem : BaseEntity
{
    public const int DISCOUNT_TIER1_MIN = 4;
    public const int DISCOUNT_TIER2_MIN = 10;
    public const int MAX_ITEM_QUANTITY = 20;
    public const decimal DISCOUNT_TIER1_RATE = 0.10m;
    public const decimal DISCOUNT_TIER2_RATE = 0.20m;

    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }
    public bool IsCancelled { get; private set; }

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

    public void Cancel()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Este item já foi cancelado.");
        IsCancelled = true;
    }

    internal void CalculateTotal()
    {
        ApplyDiscount();
        var total = Quantity * UnitPrice * (1 - Discount);
        Total = DefaultRound(total);
    }
}