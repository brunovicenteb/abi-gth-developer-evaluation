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
}