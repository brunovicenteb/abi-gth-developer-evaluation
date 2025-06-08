using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem
{
    public class CancelSaleItemValidator : AbstractValidator<CancelSaleItemCommand>
    {
        public CancelSaleItemValidator()
        {
            RuleFor(x => x.SaleId)
                .NotEmpty().WithMessage("O ID da venda é obrigatório.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("O ProductID do item da venda é obrigatório.");
        }
    }
}