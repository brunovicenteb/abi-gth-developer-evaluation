﻿using Ambev.DeveloperEvaluation.Domain.Common.Messaging;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBus _bus;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, IMessageBus bus)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _bus = bus;
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

        await _bus.PublishAsync(new SaleUpdatedEvent(updatedSale), cancellationToken);

        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}