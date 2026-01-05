using System.Linq.Expressions;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.ExpressionBuilders;

public static class FilterExpressionBuilder<T>
{
    /// <summary>
    /// Builds a LINQ expression for a given <see cref="Filter"/> to use in queries.
    /// </summary>
    /// <typeparam name="T">The entity type to filter.</typeparam>
    /// <param name="filter">The filter containing field, operator, and value.</param>
    /// <returns>An <see cref="Expression{Func}"/> representing the filter condition.</returns>
    /// <exception cref="NotSupportedException">Thrown if the filter operator is not supported.</exception>
    public static Expression<Func<T, bool>> Build(Filter filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, filter.Field);

        var targetType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
        var typedValue = Convert.ChangeType(filter.Value, targetType);

        var constant = Expression.Constant(typedValue, property.Type);

        Expression body = filter.Operator switch
        {
            FilterOperator.Equals => Expression.Equal(property, constant),
            FilterOperator.NotEquals => Expression.NotEqual(property, constant),
            FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
            FilterOperator.GreaterOrEqual => Expression.GreaterThanOrEqual(property, constant),
            FilterOperator.LessThan => Expression.LessThan(property, constant),
            FilterOperator.LessOrEqual => Expression.LessThanOrEqual(property, constant),
            FilterOperator.Contains => Expression.Call(
                property,
                nameof(string.Contains),
                Type.EmptyTypes,
                constant
            ),

            _ => throw new NotSupportedException($"Operator {filter.Operator} is not supported"),
        };

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
