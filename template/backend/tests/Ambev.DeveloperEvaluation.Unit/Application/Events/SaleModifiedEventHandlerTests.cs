using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Sales.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Events.Handlers;

public class SaleModifiedEventHandlerTests
{
    private readonly ILogger<SaleUpdatedEventHandler> _logger = Substitute.For<ILogger<SaleUpdatedEventHandler>>();
    private readonly SaleUpdatedEventHandler _handler;

    public SaleModifiedEventHandlerTests()
    {
        _handler = new SaleUpdatedEventHandler(_logger);
    }

    [Fact(DisplayName = "Should log message when sale is modified")]
    public async Task Given_SaleModifiedEvent_When_Handled_Then_LogsInformation()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var notification = new SaleUpdatedEvent(sale);

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