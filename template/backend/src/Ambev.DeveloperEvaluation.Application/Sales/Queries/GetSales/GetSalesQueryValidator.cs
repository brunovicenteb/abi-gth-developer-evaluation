using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSales;

/// <summary>
/// Valida os parâmetros da consulta paginada de vendas.
/// </summary>
public class GetSalesQueryValidator : AbstractValidator<GetSalesQuery>
{
    public GetSalesQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("O número da página deve ser maior que zero.");

        RuleFor(x => x.Size)
            .InclusiveBetween(1, 100)
            .WithMessage("O tamanho da página deve estar entre 1 e 100.");
    }
}