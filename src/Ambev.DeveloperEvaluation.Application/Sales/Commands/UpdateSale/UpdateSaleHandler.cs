using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, true, cancellationToken)
            ?? throw new KeyNotFoundException($"Venda com ID {command.Id} não encontrada.");

        var updatedSale = _mapper.Map(command, existingSale);

        updatedSale.CalculateTotal();

        await _saleRepository.UpdateAsync(updatedSale, cancellationToken);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}