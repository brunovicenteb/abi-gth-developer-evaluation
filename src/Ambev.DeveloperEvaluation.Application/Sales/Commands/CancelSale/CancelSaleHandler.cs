using Ambev.DeveloperEvaluation.Domain.Common.Messaging;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSale;

/// <summary>
/// Handler for cancel a sale.
/// </summary>
public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMessageBus _bus;

    public CancelSaleHandler(ISaleRepository saleRepository, IMessageBus bus)
    {
        _saleRepository = saleRepository;
        _bus = bus;
    }

    public async Task<CancelSaleResponse> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, false, cancellationToken)
            ?? throw new KeyNotFoundException($"Venda com ID {request.Id} não encontrada.");

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _bus.PublishAsync(new SaleCancelledEvent(sale), cancellationToken);

        return new CancelSaleResponse(true);
    }
}