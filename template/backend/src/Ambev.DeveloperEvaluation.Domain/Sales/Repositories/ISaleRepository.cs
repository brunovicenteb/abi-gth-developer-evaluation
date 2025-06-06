using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;

namespace Ambev.DeveloperEvaluation.Domain.customers.Repositories;

public interface ISaleRepository : IBaseRepository<Sale>
{
    Task<Sale> GetBySalesItemIdAsync(Guid id, CancellationToken cancellationToken);
}