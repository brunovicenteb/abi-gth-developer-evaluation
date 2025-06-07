using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

/// <summary>
/// Handler for create a sale.
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = _mapper.Map<Sale>(command);

        sale.CalculateTotal();

        await _saleRepository.AddAsync(sale, cancellationToken);

        return _mapper.Map<CreateSaleResult>(sale);
    }
}