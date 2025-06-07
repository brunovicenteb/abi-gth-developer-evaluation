using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

public class CreateSaleValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty().WithMessage(Sale.INVALID_SALE_NUMBER);

        RuleFor(x => x.CustomerName)
            .NotEmpty().WithMessage(Sale.INVALID_CUSTOMER_NAME);

        RuleFor(x => x.Branch)
            .NotEmpty().WithMessage(Sale.INVALID_BRANCH);

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage(Sale.EMPTY_SALE_ITEMS);

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.Quantity).GreaterThan(0);
            items.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}