using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Domain.Common.Filtering;

public interface IQueryParser<T> where T : BaseEntity
{
    Expression<Func<T, bool>> BuildPredicate(Dictionary<string, string> filters);
    IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, string orderBy);
}