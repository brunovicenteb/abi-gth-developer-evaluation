using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Sales.Entities;

public class Sale : BaseEntity
{
    public const string MAX_ITEM_LIMIT_EXCEEDED = "Não é permitido vender mais de 20 unidades do mesmo item.";
    public const string INVALID_DISCOUNT_RULE = "Desconto aplicado inválido para a quantidade informada.";
    public const string EMPTY_SALE_ITEMS = "Uma venda deve conter ao menos um item.";
    public const string INVALID_SALE_NUMBER = "O número da venda é obrigatório.";
    public const string INVALID_CUSTOMER_NAME = "O nome do cliente é obrigatório.";
    public const string INVALID_BRANCH = "O nome da filial é obrigatório.";

    public string SaleNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }
    public List<SaleItem> Items { get; set; } = [];

    public SaleItem CancelItemById(Guid productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId)
            ?? throw new InvalidOperationException($"Item com ProductID {productId} não encontrado.");

        item.Cancel();
        CalculateTotal();

        return item;
    }

    public void CalculateTotal()
    {
        if (!Items.Any())
            throw new InvalidOperationException(EMPTY_SALE_ITEMS);

        foreach (var item in Items.Where(o => !o.IsCancelled))
            item.CalculateTotal();

        var amount = Items.Where(o => !o.IsCancelled).Sum(i => i.Total);
        TotalAmount = DefaultRound(amount);
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}