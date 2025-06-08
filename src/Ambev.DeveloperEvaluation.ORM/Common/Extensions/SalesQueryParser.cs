using Ambev.DeveloperEvaluation.Domain.Sales.Entities;
using Ambev.DeveloperEvaluation.Domain.Sales.Filtering;

namespace Ambev.DeveloperEvaluation.ORM.Common.Extensions;

public class SalesQueryParser : QueryParser<Sale>, ISalesQueryParser
{
    public override IOrderedQueryable<Sale> ApplyOrdering(IQueryable<Sale> query, string orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            return query.OrderByDescending(s => s.CreatedAt);

        return base.ApplyOrdering(query, orderBy);
    }
}