using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken, bool applySave = true);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool applySave = true);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken, bool applySave = true);
    Task<TEntity> GetByIdAsync(Guid id, bool trackedItem, CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
    Task<(IEnumerable<TEntity> Items, int Total)> GetPagedAsync(int page, int size, string orderBy,
        Dictionary<string, string> filters, CancellationToken cancellationToken);
}