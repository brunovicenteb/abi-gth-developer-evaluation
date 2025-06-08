using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public abstract class BaseRepository<TContext, TEntity> : IBaseRepository<TEntity>
    where TContext : DbContext
    where TEntity : BaseEntity
{
    protected BaseRepository(TContext context)
    {
        Context = context;
    }

    protected TContext Context { get; }

    protected abstract DbSet<TEntity> Collection { get; }

    protected abstract string GetDefaultOrderBySearch();

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken, bool applySave = true)
    {
        var entity = await Collection.SingleOrDefaultAsync(o => o.Id == id);
        Context.Remove(entity);
        if (!applySave)
            return true;
        return await Context.SaveChangesAsync(cancellationToken) == 1;
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id, bool trackedItem, CancellationToken cancellationToken)
    {
        if (trackedItem)
            return await Collection
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        return await Collection
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken, bool applySave = true)
    {
        await Collection.AddAsync(entity);
        if (!applySave)
            return entity;
        await SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, false, cancellationToken);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool applySave = true)
    {
        var existingEntity = Collection.Find(entity.Id);
        if (existingEntity is null)
            Collection.Update(entity);
        else
        {
            Context.Entry(existingEntity).State = EntityState.Modified;
            Context.Entry(existingEntity).CurrentValues.SetValues(entity);
        }
        if (!applySave)
            return entity;
        await SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, false, cancellationToken);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken)
    {
        int count = await Context.SaveChangesAsync(cancellationToken);
        return count > 0;
    }

    public virtual IQueryable<TEntity> GetQueryableCollection()
    {
        return Collection.AsQueryable();
    }

    public async Task<(IEnumerable<TEntity> Items, int Total)> GetPagedAsync(int page, int size, string orderBy, Dictionary<string, string> filters, CancellationToken cancellationToken)
    {
        var query = GetQueryableCollection();

        query = ApplyFilters(filters, query);
        query = ApplyOrderBy(orderBy, query);

        var total = await query.CountAsync(cancellationToken);
        var skip = (page - 1) * size;

        var items = await query
            .Skip(skip)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    private  IQueryable<TEntity> ApplyOrderBy(string orderBy, IQueryable<TEntity> query)
    {
        orderBy = orderBy?.Trim() ?? GetDefaultOrderBySearch();

        var isDescending = orderBy.StartsWith("-");
        var property = isDescending ? orderBy.Substring(1) : orderBy;
        var direction = isDescending ? "descending" : "ascending";
        query = query.OrderBy($"{property} {direction}");
        return query;
    }

    private static IQueryable<TEntity> ApplyFilters(Dictionary<string, string> filters, IQueryable<TEntity> query)
    {
        if (filters is null || filters.Count == 0)
            return query;

        foreach (var filter in filters)
        {
            var value = filter.Value;
            if (!value.StartsWith("*"))
                query = query.Where($"{filter.Key} == @0", value);
            else
            {
                var trimmed = value.TrimStart('*');
                query = query.Where($"{filter.Key}.ToLower().Contains(@0)", trimmed.ToLower());
            }
        }
        return query;
    }
}