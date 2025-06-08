using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem
{
    public class CancelSaleItemRequestValidator : AbstractValidator<CancelSaleItemRequest>
    {
        public CancelSaleItemRequestValidator()
        {
            RuleFor(x => x.SaleId)
                .NotEmpty().WithMessage("O ID da venda é obrigatório.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("O ProductID do item da venda é obrigatório.");
        }
    }
}