using Ambev.DeveloperEvaluation.Domain.customers.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Customers.CreateCustomer;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(Customer.INVALID_NAME);

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage(Customer.INVALID_DOCUMENT);
    }
}