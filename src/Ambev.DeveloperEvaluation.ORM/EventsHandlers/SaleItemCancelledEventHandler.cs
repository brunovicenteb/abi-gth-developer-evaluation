using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.ORM.EventsHandlers;

public class SaleItemCancelledEventHandler : IHandleMessages<SaleItemCancelledEvent>
{
    private readonly ILogger _logger;

    public SaleItemCancelledEventHandler(ILogger<SaleItemCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleItemCancelledEvent message)
    {
        await Task.CompletedTask;
        _logger.LogInformation("[EVENT] Produto {productName} da Venda {saleNumber} cancelado.",
            message.SaleItem.ProductName, message.Sale.SaleNumber);
    }
}