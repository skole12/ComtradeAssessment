using System.Linq.Expressions;
using ComtradeAssessment.DTO;
using ComtradeAssessment.ExpressionBuilders;
using ComtradeAssessment.Extensions;
using ComtradeAssessment.Interfaces;

namespace ComtradeAssessment.Specifications;

public class BaseSpecification<T>(HashSet<string>? allowedIncludes) : ISpecification<T>
{
    public BaseSpecification(BaseRequest request, HashSet<string>? allowedIncludes = null)
        : this(allowedIncludes)
    {
        if (request == null)
            return;

        ApplyFilters(request.Filters);
        AddIncludes(request.Includes);
        ApplyPaging(request.Pagination);
        if (!string.IsNullOrWhiteSpace(request.SortBy))
            ApplySorting(request.SortBy, request.SortDescending);
    }

    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDescending { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPaginationEnabled { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = [];
    public List<string> IncludeStrings { get; } = [];
    public HashSet<string>? AllowedIncludes { get; private set; } =
        allowedIncludes?.ToHashSet(StringComparer.Ordinal);

    public void AddIncludes(string[]? includeStrings)
    {
        foreach (var include in includeStrings ?? [])
        {
            if (AllowedIncludes == null || AllowedIncludes.Contains(include))
                IncludeStrings.Add(include);
        }
    }

    protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
    {
        OrderBy = orderByExpression;
    }

    protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        OrderByDescending = orderByDescExpression;
    }

    protected void ApplyPaging(Pagination pagination)
    {
        Skip = (pagination.PageNumber - 1) * pagination.PageSize;
        Take = pagination.PageSize;
        IsPaginationEnabled = true;
    }

    protected void AddCriteria(Expression<Func<T, bool>> criteria)
    {
        if (Criteria == null)
        {
            Criteria = criteria;
            return;
        }

        Criteria = Criteria.And(criteria);
    }

    public void ApplyFilters(IEnumerable<Filter> filters)
    {
        if (filters != null)
            foreach (var filter in filters)
            {
                AddCriteria(FilterExpressionBuilder<T>.Build(filter));
            }
    }

    /// <summary>
    /// Applies sorting to the query based on the specified property name and sort direction.
    /// </summary>
    /// <param name="sortBy">The name of the property to sort by.</param>
    /// <param name="descending">Indicates whether the sorting should be in descending order.</param>
    protected void ApplySorting(string sortBy, bool descending)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sortBy);
        var conversion = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<T, object>>(conversion, parameter);

        if (descending)
            AddOrderByDescending(lambda);
        else
            AddOrderBy(lambda);
    }
}
