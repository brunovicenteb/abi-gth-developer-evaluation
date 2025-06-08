using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.ORM.EventsHandlers;

public class SaleUpdatedEventHandler : IHandleMessages<SaleUpdatedEvent>
{
    private readonly ILogger _logger;

    public SaleUpdatedEventHandler(ILogger<SaleUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleUpdatedEvent message)
    {
        await Task.CompletedTask;
        _logger.LogInformation("[EVENT] Venda {saleNumber} atualizada.", message.Sale.SaleNumber);
    }
}