using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SalesRepository : BaseRepository<DefaultContext, Sale>, ISaleRepository
{
    public SalesRepository(DefaultContext context) : base(context)
    {
    }

    protected override DbSet<Sale> Collection => Context.Sales;

    public override async Task<Sale> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await Collection
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Sale> GetBySalesItemIdAsync(Guid salesItemId, CancellationToken cancellationToken)
    {
        return await Collection
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                o => o.Items.Any(i => i.Id == salesItemId),
                cancellationToken
            );
    }
}