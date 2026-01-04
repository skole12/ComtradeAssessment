using System.Linq.Expressions;

namespace ComtradeAssessment.Extensions;

public static class ExpressionExtensions
{
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
