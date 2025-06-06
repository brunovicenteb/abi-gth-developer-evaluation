using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public record CancelSaleItemCommand(Guid SaleItemId) : IRequest<CancelSaleItemResult>;