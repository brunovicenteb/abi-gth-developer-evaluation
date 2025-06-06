using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResult>
{
    private readonly ISaleRepository _saleRepository;

    public CancelSaleItemHandler(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
    }

    public async Task<CancelSaleItemResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetBySalesItemIdAsync(request.SaleItemId, cancellationToken)
            ?? throw new KeyNotFoundException($"Item de venda com ID {request.SaleItemId} não encontrada.");

        sale.CancelItemById(request.SaleItemId);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        return new CancelSaleItemResult(true);
    }
}