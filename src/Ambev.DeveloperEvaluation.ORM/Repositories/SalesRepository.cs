using Ambev.DeveloperEvaluation.Domain.customers.Repositories;
using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Domain.Sales.Filtering;
using Ambev.DeveloperEvaluation.ORM.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SalesRepository : BaseRepository<DefaultContext, Sale>, ISaleRepository
{
    public SalesRepository(DefaultContext context, ISalesQueryParser queryParser)
        : base(context, queryParser)
    {
    }

    protected override QueryParser<Sale> CreateParser() => new SalesQueryParser();

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