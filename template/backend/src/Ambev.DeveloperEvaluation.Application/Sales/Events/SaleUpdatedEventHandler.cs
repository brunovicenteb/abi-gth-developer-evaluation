using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public class SaleUpdatedEventHandler : INotificationHandler<SaleUpdatedEvent>
{
    private readonly ILogger _logger;

    public SaleUpdatedEventHandler(ILogger<SaleUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleUpdatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[EVENT] Venda {saleNumber} atualizada.", notification.Sale.SaleNumber);
        return Task.CompletedTask;
    }
}