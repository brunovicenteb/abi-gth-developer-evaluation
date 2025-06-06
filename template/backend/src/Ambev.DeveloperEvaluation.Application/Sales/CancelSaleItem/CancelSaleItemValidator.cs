using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem
{
    public class CancelSaleItemValidator : AbstractValidator<CancelSaleItemCommand>
    {
        public CancelSaleItemValidator()
        {
            RuleFor(x => x.SaleItemId)
                .NotEmpty().WithMessage("O ID do item da venda é obrigatório.");
        }
    }
}