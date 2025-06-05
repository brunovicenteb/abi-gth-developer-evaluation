using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.customers.Entities;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.CreateCustomer;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CreateCustomerHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCustomerValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var customer = _mapper.Map<Customer>(command);
        var createdCustomer = await _customerRepository.AddAsync(customer, cancellationToken);

        var result = _mapper.Map<CreateCustomerResult>(createdCustomer);
        return result;
    }
}