using System.Linq.Expressions;

namespace ComtradeAssessment.Extensions;

public static class ExpressionExtensions
{
    /// <summary>
    /// Combines two expressions with a logical AND, returning a new expression that is true
    /// only if both input expressions are true.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expressions.</typeparam>
    /// <param name="left">The first expression.</param>
    /// <param name="right">The second expression.</param>
    /// <returns>A new expression representing the logical AND of the two input expressions.</returns>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right
    )
    {
        var parameter = Expression.Parameter(typeof(T), "x");

        var leftBody = Expression.Invoke(left, parameter);
        var rightBody = Expression.Invoke(right, parameter);

        var body = Expression.AndAlso(leftBody, rightBody);

        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
