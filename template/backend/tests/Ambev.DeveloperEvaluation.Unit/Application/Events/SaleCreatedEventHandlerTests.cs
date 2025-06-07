using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Sales.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events.Handlers;

public class SaleCreatedEventHandlerTests
{
    private readonly ILogger<SaleCreatedEventHandler> _logger = Substitute.For<ILogger<SaleCreatedEventHandler>>();
    private readonly SaleCreatedEventHandler _handler;

    public SaleCreatedEventHandlerTests()
    {
        _handler = new SaleCreatedEventHandler(_logger);
    }

    [Fact(DisplayName = "Should log message when sale is created")]
    public async Task Given_SaleCreatedEvent_When_Handled_Then_LogsInformation()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var notification = new SaleCreatedEvent(sale);

        // Act
        await _handler.Handle(notification, CancellationToken.None);

        // Assert
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains(sale.SaleNumber)),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }
}