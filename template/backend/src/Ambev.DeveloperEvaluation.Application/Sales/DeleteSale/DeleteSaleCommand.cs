using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

/// <summary>
/// Command to request the deletion of a sale.
/// </summary>
public record DeleteSaleCommand(Guid Id) : IRequest<DeleteSaleResponse>;