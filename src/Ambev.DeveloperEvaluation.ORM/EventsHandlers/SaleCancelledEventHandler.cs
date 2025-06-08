using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.ORM.EventsHandlers;

public class SaleCancelledEventHandler : IHandleMessages<SaleCancelledEvent>
{
    private readonly ILogger _logger;

    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleCancelledEvent message)
    {
        await Task.CompletedTask;
        _logger.LogInformation("[EVENT] Venda {saleNumber} cancelada.", message.Sale.SaleNumber);
    }
}