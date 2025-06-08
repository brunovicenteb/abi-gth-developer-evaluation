using System.Linq.Expressions;
using System.Reflection;

namespace Ambev.DeveloperEvaluation.ORM.Common.Filtering;

public static class QueryableExtensions
{
    private const string ORDER_BY = "OrderBy";
    private const string ORDER_BY_DESC = "OrderByDescending";
    private const string THEN_BY = "ThenBy";
    private const string THEN_BY_DESC = "ThenByDescending";

    public static IOrderedQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName)
        => ApplyOrdering(source, propertyName, ORDER_BY);

    public static IOrderedQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> source, string propertyName)
        => ApplyOrdering(source, propertyName, ORDER_BY_DESC);

    public static IOrderedQueryable<T> ThenByDynamic<T>(this IOrderedQueryable<T> source, string propertyName)
        => ApplyOrdering(source, propertyName, THEN_BY);

    public static IOrderedQueryable<T> ThenByDescendingDynamic<T>(this IOrderedQueryable<T> source, string propertyName)
        => ApplyOrdering(source, propertyName, THEN_BY_DESC);

    private static IOrderedQueryable<T> ApplyOrdering<T>(IQueryable<T> source, string propertyName, string methodName)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T).Name}'");

        var propertyAccess = Expression.Property(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var result = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName
                        && m.IsGenericMethodDefinition
                        && m.GetGenericArguments().Length == 2
                        && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.PropertyType)
            .Invoke(null, [source, orderByExpression]);

        return (IOrderedQueryable<T>)result;
    }
}