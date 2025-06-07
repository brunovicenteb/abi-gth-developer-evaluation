using Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.Queries.GetSales.TestData;
using AutoMapper;
using FluentValidation;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Queries.GetSales;

public class GetSalesHandlerTests
{
    private readonly ISaleRepository _repository = Substitute.For<ISaleRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _handler = new GetSalesHandler(_repository, _mapper);
    }

    [Fact(DisplayName = "Should return paged result when command is valid")]
    public async Task Given_ValidCommand_When_Handled_Then_ReturnsPagedResult()
    {
        // Arrange
        var command = GetSalesTestData.GenerateValidCommand();
        var domainList = GetSalesTestData.GenerateValidSalesList();
        var mapped = GetSalesTestData.GeneratePagedResult(domainList);

        _repository.GetPagedAsync(
            command.Page,
            command.Size,
            command.OrderBy,
            Arg.Any<Dictionary<string, string>>(),
            Arg.Any<CancellationToken>())
        .Returns((domainList, domainList.Count));

        _mapper.Map<List<SaleListItemDto>>(domainList).Returns(mapped.Data);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domainList.Count, result.Total);
        Assert.Equal(mapped.Data.Count, result.Data.Count);
    }

    [Fact(DisplayName = "Should throw exception when repository fails")]
    public async Task Given_RepositoryFails_When_Handled_Then_ThrowsException()
    {
        // Arrange
        var command = GetSalesTestData.GenerateValidCommand();

        _repository.GetPagedAsync(
            command.Page,
            command.Size,
            command.OrderBy,
            Arg.Any<Dictionary<string, string>>(),
            Arg.Any<CancellationToken>())
        .Throws(new Exception("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact(DisplayName = "Should throw validation exception when command is invalid")]
    public async Task Given_InvalidCommand_When_Handled_Then_ThrowsValidationException()
    {
        // Arrange
        var command = GetSalesTestData.GenerateInvalidCommand();

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
