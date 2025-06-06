using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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
}