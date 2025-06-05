using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken, bool applySave = true);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool applySave = true);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken, bool applySave = true);
    Task<TEntity> GetIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
}