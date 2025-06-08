using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.ORM.EventsHandlers;

public class SaleCreatedEventHandler : IHandleMessages<SaleCreatedEvent>
{
    private readonly ILogger _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleCreatedEvent message)
    {
        await Task.CompletedTask;
        _logger.LogInformation("[EVENT] Venda {saleNumber} criada.", message.Sale.SaleNumber);
    }
}