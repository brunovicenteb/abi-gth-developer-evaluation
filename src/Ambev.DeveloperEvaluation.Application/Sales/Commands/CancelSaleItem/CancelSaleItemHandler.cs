using Ambev.DeveloperEvaluation.Domain.Common.Messaging;
using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Events;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMessageBus _bus;

    public CancelSaleItemHandler(ISaleRepository saleRepository, IMessageBus bus)
    {
        _saleRepository = saleRepository;
        _bus = bus;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, true, cancellationToken)
            ?? throw new KeyNotFoundException($"Venda com ID {request.SaleId} não encontrada.");

        var saleItem = sale.CancelItemById(request.ProductId);
        await _saleRepository.UpdateAsync(sale, cancellationToken);

        await _bus.PublishAsync(new SaleItemCancelledEvent(sale, saleItem), cancellationToken);

        return new CancelSaleItemResult(true);
    }
}