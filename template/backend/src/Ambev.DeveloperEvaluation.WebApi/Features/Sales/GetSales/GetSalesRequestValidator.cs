using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales
{
    public class GetSalesRequestValidator : AbstractValidator<GetSalesRequest>
    {
        public GetSalesRequestValidator()
        {
            RuleFor(x => x.Page).GreaterThan(0).WithMessage("Página deve ser maior que 0.");
            RuleFor(x => x.Size).InclusiveBetween(1, 100).WithMessage("Tamanho da página deve estar entre 1 e 100.");
        }
    }
}