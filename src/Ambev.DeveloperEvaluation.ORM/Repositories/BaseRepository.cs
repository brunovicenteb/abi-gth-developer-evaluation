using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Common.Filtering;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM.Common.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public abstract class BaseRepository<TContext, TEntity> : IBaseRepository<TEntity>
    where TContext : DbContext
    where TEntity : BaseEntity
{
    private readonly IQueryParser<TEntity> _queryParser;

    protected BaseRepository(TContext context, IQueryParser<TEntity> queryParser)
    {
        Context = context;
        _queryParser = queryParser;
    }

    protected TContext Context { get; }

    protected abstract DbSet<TEntity> Collection { get; }

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

    protected abstract QueryParser<TEntity> CreateParser();

    public async Task<(IEnumerable<TEntity> Items, int Total)> GetPagedAsync(int page, int size, string orderBy,
        Dictionary<string, string> filters, CancellationToken cancellationToken)
    {
        var query = GetQueryableCollection();

        var predicate = _queryParser.BuildPredicate(filters);
        var filtered = query.Where(predicate);
        var ordered = _queryParser.ApplyOrdering(filtered, orderBy);

        var total = await ordered.CountAsync(cancellationToken);
        var result = await ordered
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (result, total);
    }
}