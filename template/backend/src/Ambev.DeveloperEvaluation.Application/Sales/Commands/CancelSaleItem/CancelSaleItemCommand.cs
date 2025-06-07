using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CancelSaleItem;

public record CancelSaleItemCommand(Guid SaleId, Guid ProductId) : IRequest<CancelSaleItemResult>;