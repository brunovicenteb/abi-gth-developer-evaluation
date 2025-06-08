using Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.UpdateSale.TestData;
using AutoMapper;
using FluentValidation;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Commands.UpdateSale;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _repository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _handler = new UpdateSaleHandler(_repository, _mapper);
    }

    [Theory(DisplayName = "Should return result when update command is valid")]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task Given_ValidCommand_When_Handled_Then_ReturnsResult(int productAmount)
    {
        // Arrange
        var command = UpdateSaleTestData.GenerateValidCommand(productAmount);
        var sale = UpdateSaleTestData.GenerateValidDomainEntity(command);
        var expected = UpdateSaleTestData.GenerateResult(sale);

        _repository.GetByIdAsync(command.Id, true, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map(command, sale).Returns(sale);
        _repository.UpdateAsync(sale, Arg.Any<CancellationToken>(), true).Returns(sale);
        _mapper.Map<UpdateSaleResult>(sale).Returns(expected);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.SaleNumber, result.SaleNumber);
        Assert.Equal(expected.TotalAmount, result.TotalAmount);
    }

    [Theory(DisplayName = "Should throw when repository update fails")]
    [InlineData(2)]
    public async Task Given_ValidCommand_When_RepositoryFails_Then_Throws(int productAmount)
    {
        // Arrange
        var command = UpdateSaleTestData.GenerateValidCommand(productAmount);
        var sale = UpdateSaleTestData.GenerateValidDomainEntity(command);

        _repository.GetByIdAsync(command.Id, true, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map(command, sale).Returns(sale);
        _repository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>(), true)
            .Throws(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw validation exception when command is invalid")]
    public async Task Given_InvalidCommand_When_Handled_Then_ThrowsValidationException()
    {
        // Arrange
        var command = UpdateSaleTestData.GenerateInvalidCommandWithoutItems();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
