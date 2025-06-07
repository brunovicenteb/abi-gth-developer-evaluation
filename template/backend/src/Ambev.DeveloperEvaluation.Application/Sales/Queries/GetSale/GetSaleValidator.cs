using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Queries.GetSale;

public class GetSaleValidator : AbstractValidator<GetSaleQuery>
{
    public GetSaleValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O ID da venda é obrigatório.");
    }
}