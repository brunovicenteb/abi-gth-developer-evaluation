using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SaleItemCancelledEventHandler : INotificationHandler<SaleItemCancelledEvent>
{
    private readonly ILogger _logger;

    public SaleItemCancelledEventHandler(ILogger<SaleItemCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[EVENT] Produto {productName} da Venda {saleNumber} cancelado.",
            notification.SaleItem.ProductName, notification.Sale.SaleNumber);
        return Task.CompletedTask;
    }
}