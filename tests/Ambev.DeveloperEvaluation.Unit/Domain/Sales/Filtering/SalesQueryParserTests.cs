using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.ORM.Common.Extensions;
using Bogus;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales.Filtering;

public class SalesQueryParserTests
{
    private readonly SalesQueryParser _parser = new();
    private readonly List<Sale> _sales;

    public SalesQueryParserTests()
    {
        var faker = new Faker<Sale>()
            .RuleFor(s => s.Id, f => Guid.NewGuid())
            .RuleFor(s => s.CustomerId, f => f.Random.Guid())
            .RuleFor(s => s.Branch, f => f.Company.CompanyName())
            .RuleFor(s => s.TotalAmount, f => f.Random.Decimal(100, 1000))
            .RuleFor(s => s.CreatedAt, f => f.Date.Past(1))
            .RuleFor(s => s.IsCancelled, _ => false);

        _sales = faker.Generate(50);
    }

    [Fact]
    public void BuildPredicate_Should_Filter_By_Exact_CustomerId()
    {
        // Arrange
        var target = _sales.First();
        var filters = new Dictionary<string, string>
        {
            { "CustomerId", target.CustomerId.ToString() }
        };

        // Act
        var predicate = _parser.BuildPredicate(filters);
        var result = _sales.AsQueryable().Where(predicate).ToList();

        // Assert
        result.Should().OnlyContain(s => s.CustomerId == target.CustomerId);
    }

    [Fact]
    public void BuildPredicate_Should_Filter_By_MinTotal()
    {
        // Arrange
        var filters = new Dictionary<string, string>
        {
            { "_minTotalAmount", "500" }
        };

        // Act
        var predicate = _parser.BuildPredicate(filters);
        var result = _sales.AsQueryable().Where(predicate).ToList();

        // Assert
        result.Should().OnlyContain(s => s.TotalAmount >= 500);
    }

    [Fact]
    public void BuildPredicate_Should_Filter_By_Contains()
    {
        // Arrange
        var target = _sales.First();
        var partial = target.Branch.Substring(1, 3); // Meio do texto
        var filters = new Dictionary<string, string>
        {
            { "Branch", "*" + partial + "*" }
        };

        // Act
        var predicate = _parser.BuildPredicate(filters);
        var result = _sales.AsQueryable().Where(predicate).ToList();

        // Assert
        result.Should().Contain(x => x.Branch.Contains(partial));
    }

    [Fact]
    public void ApplyOrdering_Should_Order_By_Total_Then_By_CreatedAt()
    {
        // Arrange
        var orderBy = "TotalAmount desc, CreatedAt asc";

        // Act
        var result = _parser.ApplyOrdering(_sales.AsQueryable(), orderBy).ToList();

        // Assert
        result.Should().BeInDescendingOrder(x => x.TotalAmount);
    }

    [Fact]
    public void ApplyOrdering_Should_Default_To_CreatedAt_When_Empty()
    {
        // Act
        var result = _parser.ApplyOrdering(_sales.AsQueryable(), "").ToList();

        // Assert
        result.Should().BeInDescendingOrder(x => x.CreatedAt);
    }
}