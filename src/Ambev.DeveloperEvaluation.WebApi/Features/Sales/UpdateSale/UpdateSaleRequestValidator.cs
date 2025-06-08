using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID da venda é obrigatório.");
        RuleFor(x => x.SaleNumber).NotEmpty().WithMessage(Sale.INVALID_SALE_NUMBER);
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage(Sale.INVALID_CUSTOMER_NAME);
        RuleFor(x => x.Branch).NotEmpty().WithMessage(Sale.INVALID_BRANCH);
        RuleFor(x => x.Items).NotEmpty().WithMessage(Sale.EMPTY_SALE_ITEMS);

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.Quantity).GreaterThan(0);
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}