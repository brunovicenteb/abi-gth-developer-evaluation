using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Sales.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events.Handlers;

public class SaleCancelledEventHandlerTests
{
    private readonly ILogger<SaleCancelledEventHandler> _logger = Substitute.For<ILogger<SaleCancelledEventHandler>>();
    private readonly SaleCancelledEventHandler _handler;

    public SaleCancelledEventHandlerTests()
    {
        _handler = new SaleCancelledEventHandler(_logger);
    }

    [Fact(DisplayName = "Should log message when sale is cancelled")]
    public async Task Given_SaleCancelledEvent_When_Handled_Then_LogsInformation()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var notification = new SaleCancelledEvent(sale);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains(sale.SaleNumber.ToString())),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}