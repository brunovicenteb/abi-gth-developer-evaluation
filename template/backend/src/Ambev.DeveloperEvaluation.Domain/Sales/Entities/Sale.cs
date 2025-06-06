using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using MediatR;
using System.Threading;

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

    public void CancelItemById(Guid saleItemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == saleItemId);
        if (item == null)
            throw new InvalidOperationException($"Item com ID {saleItemId} não encontrado.");

        item.Cancel();
        CalculateTotal();
    }

    public void CalculateTotal()
    {
        if (!Items.Any())
            throw new InvalidOperationException(EMPTY_SALE_ITEMS);

        foreach (var item in Items)
            item.CalculateTotal();

        TotalAmount = Items.Sum(i => i.Total);
    }

    public void Cancel()
    {
        IsCancelled = true;
    }
}