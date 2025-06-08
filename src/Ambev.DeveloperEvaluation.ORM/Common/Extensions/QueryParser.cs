using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Common.Filtering;
using Ambev.DeveloperEvaluation.ORM.Common.Filtering;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ambev.DeveloperEvaluation.ORM.Common.Extensions;

public abstract class QueryParser<T> : IQueryParser<T>
    where T : BaseEntity
{
    protected virtual bool ShouldIgnoreFilter(string key) => false;

    protected virtual string MapPropertyName(string queryKey) => queryKey;

    public virtual Expression<Func<T, bool>> BuildPredicate(Dictionary<string, string> filters)
    {
        if (filters == null || filters.Count == 0)
            return e => true;

        var parameter = Expression.Parameter(typeof(T), "e");
        Expression body = null;

        foreach (var filter in filters)
        {
            if (ShouldIgnoreFilter(filter.Key))
                continue;

            var predicate = BuildSinglePredicate(filter.Key, filter.Value, parameter);
            if (predicate != null)
                body = body == null ? predicate : Expression.AndAlso(body, predicate);
        }

        return body == null ? e => true : Expression.Lambda<Func<T, bool>>(body, parameter);
    }

    private Expression BuildSinglePredicate(string key, string value, ParameterExpression parameter)
    {
        var entityType = typeof(T);

        var minMatch = Regex.Match(key, "^_min(.*)", RegexOptions.IgnoreCase);
        var maxMatch = Regex.Match(key, "^_max(.*)", RegexOptions.IgnoreCase);

        if (minMatch.Success || maxMatch.Success)
        {
            var propName = MapPropertyName(minMatch.Success ? minMatch.Groups[1].Value : maxMatch.Groups[1].Value);
            var prop = entityType.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop == null) return null;

            var member = Expression.Property(parameter, prop);
            var constant = Expression.Constant(Convert.ChangeType(value, prop.PropertyType));

            return minMatch.Success
                ? Expression.GreaterThanOrEqual(member, constant)
                : Expression.LessThanOrEqual(member, constant);
        }

        var cleanKey = MapPropertyName(key);
        var propInfo = entityType.GetProperty(cleanKey, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (propInfo != null && propInfo.PropertyType == typeof(string))
        {
            var member = Expression.Property(parameter, propInfo);
            var likeValue = value.Trim('*');
            var constant = Expression.Constant(likeValue);

            MethodInfo method = value.StartsWith("*") && value.EndsWith("*")
                ? typeof(string).GetMethod("Contains", [typeof(string)])
                : value.StartsWith("*")
                    ? typeof(string).GetMethod("EndsWith", [typeof(string)])
                    : value.EndsWith("*")
                        ? typeof(string).GetMethod("StartsWith", [typeof(string)])
                        : typeof(string).GetMethod("Equals", [typeof(string)]);

            return method != null ? Expression.Call(member, method, constant) : null;
        }

        if (propInfo != null)
        {
            var member = Expression.Property(parameter, propInfo);
            var constant = Expression.Constant(Convert.ChangeType(value, propInfo.PropertyType));
            return Expression.Equal(member, constant);
        }

        return null;
    }

    public virtual IOrderedQueryable<T> ApplyOrdering(IQueryable<T> query, string orderBy)
    {
        if (string.IsNullOrWhiteSpace(orderBy))
            throw new ArgumentException("Ordering must be provided or explicitly handled in derived class.");

        var parts = orderBy.Split(',').Select(p => p.Trim()).Where(p => !string.IsNullOrEmpty(p));
        bool isFirst = true;
        IOrderedQueryable<T> result = null;

        foreach (var part in parts)
        {
            var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = MapPropertyName(tokens[0]);
            var dir = tokens.Length > 1 ? tokens[1].ToLower() : "asc";

            if (isFirst)
            {
                result = dir == "desc"
                    ? query.OrderByDescendingDynamic(field)
                    : query.OrderByDynamic(field);
                isFirst = false;
            }
            else if (result != null)
            {
                result = dir == "desc"
                    ? result.ThenByDescendingDynamic(field)
                    : result.ThenByDynamic(field);
            }
        }

        return result ?? throw new InvalidOperationException("Ordering could not be applied.");
    }
}