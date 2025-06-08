namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem
{
    public record CancelSaleItemRequest(Guid SaleId, Guid ProductId);
}