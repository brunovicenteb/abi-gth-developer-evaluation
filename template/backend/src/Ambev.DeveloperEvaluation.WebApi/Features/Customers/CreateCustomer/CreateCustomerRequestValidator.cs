using Ambev.DeveloperEvaluation.Domain.customers.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Customers.CreateCustomer;

public class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Customer.INVALID_NAME);

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage(Customer.INVALID_DOCUMENT);
    }
}