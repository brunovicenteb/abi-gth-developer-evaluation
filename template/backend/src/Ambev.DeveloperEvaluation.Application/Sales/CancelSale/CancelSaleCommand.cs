using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Command to request the deletion of a sale.
/// </summary>
public record CancelSaleCommand(Guid Id) : IRequest<CancelSaleResponse>;