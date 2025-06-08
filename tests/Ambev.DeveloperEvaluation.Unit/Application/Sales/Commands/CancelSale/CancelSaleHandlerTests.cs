using Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Common.Messaging;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CancelSale.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CancelSale;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _repository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly CancelSaleHandler _handler;
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();

    public CancelSaleHandlerTests()
    {
        _handler = new CancelSaleHandler(_repository, _bus);
    }

    [Fact(DisplayName = "Should return result when cancel command is valid")]
    public async Task Given_ValidCommand_When_Handled_Then_ReturnsResult()
    {
        // Arrange
        var command = CancelSaleTestData.GenerateValidCommand();
        var domain = CancelSaleTestData.GenerateValidDomainEntity(command);
        var result = CancelSaleTestData.GenerateResult();

        _repository.GetByIdAsync(command.Id, false, Arg.Any<CancellationToken>()).Returns(domain);
        _repository.UpdateAsync(domain, Arg.Any<CancellationToken>(), true).Returns(domain);
        _mapper.Map<CancelSaleResponse>(domain).Returns(result);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    [Fact(DisplayName = "Should throw when sale is not found")]
    public async Task Given_NonExistentSale_When_Handled_Then_ThrowsKeyNotFound()
    {
        // Arrange
        var command = CancelSaleTestData.GenerateValidCommand();
        _repository.GetByIdAsync(command.Id, false, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw when repository throws exception")]
    public async Task Given_RepositoryFails_When_Handled_Then_ThrowsException()
    {
        // Arrange
        var command = CancelSaleTestData.GenerateValidCommand();
        var domain = CancelSaleTestData.GenerateValidDomainEntity(command);

        _repository.GetByIdAsync(command.Id, false, Arg.Any<CancellationToken>()).Returns(domain);
        _repository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>(), true)
            .Throws(new Exception("Falha no banco"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw validation exception when command is invalid")]
    public async Task Given_InvalidCommand_When_Handled_Then_ThrowsValidationException()
    {
        // Arrange
        var command = CancelSaleTestData.GenerateInvalidCommand();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}