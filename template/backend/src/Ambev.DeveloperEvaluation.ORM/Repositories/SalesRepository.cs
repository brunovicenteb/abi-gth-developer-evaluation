using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SalesRepository : BaseRepository<DefaultContext, Sale>, ISaleRepository
{
    private const string DEFAULT_ORDER_BY = "-CreatedAt";

    public SalesRepository(DefaultContext context) : base(context)
    {
    }

    protected override string GetDefaultOrderBySearch() => DEFAULT_ORDER_BY;

    protected override DbSet<Sale> Collection => Context.Sales;

    public override IQueryable<Sale> GetQueryableCollection()
    {
        return Collection
            .Where(sale => !sale.IsCancelled)
            .AsNoTracking();
    }

    public override async Task<Sale> GetByIdAsync(Guid id, bool trackedItem, CancellationToken cancellationToken)
    {
        if (trackedItem)
            return await Collection
                .Where(sale => !sale.IsCancelled && sale.Id == id)
                .Include(sale => sale.Items.Where(item => !item.IsCancelled))
                .FirstOrDefaultAsync(cancellationToken);
        return await Collection
            .Where(sale => !sale.IsCancelled && sale.Id == id)
            .Include(sale => sale.Items.Where(item => !item.IsCancelled))
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}