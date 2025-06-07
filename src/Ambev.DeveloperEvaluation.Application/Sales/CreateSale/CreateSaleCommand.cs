using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Commands.CreateSale;

public class CreateSaleCommand : IRequest<CreateSaleResult>
{
    public string SaleNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public List<CreateSaleItemDto> Items { get; set; } = [];
}