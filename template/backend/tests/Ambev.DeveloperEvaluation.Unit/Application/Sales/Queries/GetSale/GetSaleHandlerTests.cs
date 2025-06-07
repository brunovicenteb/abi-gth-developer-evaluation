using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Queries.GetSale.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Queries.GetSale;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _repository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _handler = new GetSaleHandler(_repository, _mapper);
    }

    [Fact(DisplayName = "Should return result when command is valid")]
    public async Task Given_ValidCommand_When_Handled_Then_ReturnsResult()
    {
        // Arrange
        var command = GetSaleTestData.GenerateValidCommand();
        var domain = GetSaleTestData.GenerateValidDomainEntity(command);
        var expected = GetSaleTestData.GenerateResult(domain);

        _repository.GetByIdAsync(command.Id, false, Arg.Any<CancellationToken>()).Returns(domain);
        _mapper.Map<GetSaleResult>(domain).Returns(expected);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.SaleNumber, result.SaleNumber);
        Assert.Equal(expected.TotalAmount, result.TotalAmount);
    }

    [Fact(DisplayName = "Should throw when sale not found")]
    public async Task Given_NonExistentSale_When_Handled_Then_ThrowsKeyNotFound()
    {
        // Arrange
        var command = GetSaleTestData.GenerateValidCommand();
        _repository.GetByIdAsync(command.Id, false, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw when repository throws exception")]
    public async Task Given_RepositoryFails_When_Handled_Then_ThrowsException()
    {
        // Arrange
        var command = GetSaleTestData.GenerateValidCommand();

        _repository.GetByIdAsync(Arg.Any<Guid>(), false, Arg.Any<CancellationToken>())
            .Throws(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw validation exception when command is invalid")]
    public async Task Given_InvalidCommand_When_Handled_Then_ThrowsValidationException()
    {
        // Arrange
        var command = GetSaleTestData.GenerateInvalidCommand();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
