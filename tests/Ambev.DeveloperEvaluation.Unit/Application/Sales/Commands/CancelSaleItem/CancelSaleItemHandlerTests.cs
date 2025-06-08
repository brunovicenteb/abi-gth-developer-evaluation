using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Common.Messaging;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CancelSaleItem.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CancelSaleItem;

public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _repository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _handler = new CancelSaleItemHandler(_repository, _bus);
    }

    [Fact(DisplayName = "Should return result when cancel item command is valid")]
    public async Task Given_ValidCommand_When_Handled_Then_ReturnsResult()
    {
        // Arrange
        var command = CancelSaleItemTestData.GenerateValidCommand();
        var domain = CancelSaleItemTestData.GenerateValidDomainEntity(command);
        var result = CancelSaleItemTestData.GenerateResult(domain);

        _repository.GetByIdAsync(command.SaleId, true, Arg.Any<CancellationToken>()).Returns(domain);
        _repository.UpdateAsync(domain, Arg.Any<CancellationToken>(), true).Returns(domain);
        _mapper.Map<CancelSaleItemResult>(domain).Returns(result);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact(DisplayName = "Should throw when item does not exist in sale")]
    public async Task Given_InvalidItemId_When_Handled_Then_Throws()
    {
        // Arrange
        var command = CancelSaleItemTestData.GenerateValidCommand();
        var domain = CancelSaleItemTestData.GenerateValidDomainEntity(command, includeItem: false);

        _repository.GetByIdAsync(command.SaleId, true, Arg.Any<CancellationToken>()).Returns(domain);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw when sale not found")]
    public async Task Given_NonExistentSale_When_Handled_Then_ThrowsKeyNotFound()
    {
        // Arrange
        var command = CancelSaleItemTestData.GenerateValidCommand();
        _repository.GetByIdAsync(command.SaleId, true, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw when repository fails")]
    public async Task Given_RepositoryFails_When_Handled_Then_ThrowsException()
    {
        // Arrange
        var command = CancelSaleItemTestData.GenerateValidCommand();
        var domain = CancelSaleItemTestData.GenerateValidDomainEntity(command);

        _repository.GetByIdAsync(command.SaleId, true, Arg.Any<CancellationToken>()).Returns(domain);
        _repository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>(), true)
            .Throws(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw validation exception when command is invalid")]
    public async Task Given_InvalidCommand_When_Handled_Then_ThrowsValidationException()
    {
        // Arrange
        var command = CancelSaleItemTestData.GenerateInvalidCommand();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}