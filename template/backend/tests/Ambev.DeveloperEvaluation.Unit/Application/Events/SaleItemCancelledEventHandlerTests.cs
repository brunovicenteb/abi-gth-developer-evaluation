using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Sales.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events.Handlers;

public class SaleItemCancelledEventHandlerTests
{
    private readonly ILogger<SaleItemCancelledEventHandler> _logger = Substitute.For<ILogger<SaleItemCancelledEventHandler>>();
    private readonly SaleItemCancelledEventHandler _handler;

    public SaleItemCancelledEventHandlerTests()
    {
        _handler = new SaleItemCancelledEventHandler(_logger);
    }

    [Fact(DisplayName = "Should log message when sale item is cancelled")]
    public async Task Given_SaleItemCancelledEvent_When_Handled_Then_LogsInformation()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var saleItem = sale.Items.First();
        var notification = new SaleItemCancelledEvent(sale, saleItem);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o =>
                o.ToString()!.Contains(sale.SaleNumber.ToString()) &&
                o.ToString()!.Contains(saleItem.ProductName.ToString())
            ),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}