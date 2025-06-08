using Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Common.Messaging;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CreateSale.TestData;
using AutoMapper;
using FluentValidation;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.CreateSale;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _repository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _handler = new CreateSaleHandler(_repository, _mapper, _bus);
    }

    [Theory(DisplayName = "Should return result when command is valid")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Given_ValidCommand_When_Handled_Then_ReturnsResult(int productAmount)
    {
        // Arrange
        var command = CreateSaleTestData.GenerateValidCommand(productAmount);
        var domainSale = CreateSaleTestData.GenerateValidDomainEntity(command);
        var expected = CreateSaleTestData.GenerateResult(domainSale);

        _mapper.Map<Sale>(command).Returns(domainSale);
        _repository.AddAsync(domainSale, Arg.Any<CancellationToken>(), true).Returns(domainSale);
        _mapper.Map<CreateSaleResult>(domainSale).Returns(expected);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.SaleNumber, result.SaleNumber);
        Assert.Equal(expected.TotalAmount, result.TotalAmount);
    }

    [Theory(DisplayName = "Should throw when repository throws")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Given_ValidCommand_When_RepositoryFails_Then_Throws(int productAmount)
    {
        // Arrange
        var command = CreateSaleTestData.GenerateValidCommand(productAmount);
        var domainSale = CreateSaleTestData.GenerateValidDomainEntity(command);

        _mapper.Map<Sale>(command).Returns(domainSale);
        _repository.AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>(), true)
            .Throws(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw validation exception when command is invalid")]
    public async Task Given_InvalidCommand_When_Handled_Then_ThrowsValidationException()
    {
        // Arrange
        var command = CreateSaleTestData.GenerateInvalidCommandWithoutItems();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}