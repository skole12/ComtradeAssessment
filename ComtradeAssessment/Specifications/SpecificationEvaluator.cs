using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Specifications;

public static class SpecificationEvaluator<T>
    where T : class
{
    /// <summary>
    /// Builds and returns a query based on the provided specification, applying filters, includes, sorting, distinct, and pagination options.
    /// </summary>
    /// <param name="query">The base query to apply the specification to.</param>
    /// <param name="spec">The specification containing query conditions, includes, and sorting options.</param>
    /// <returns>Returns an <see cref="IQueryable{T}"/> representing the filtered and configured query.</returns>
    public static IQueryable<T> GetQuery(IQueryable<T> query, ISpecification<T> spec)
    {
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));

        query = spec.IncludeStrings.Aggregate(
            query,
            (current, include) => current.Include(include)
        );

        if (spec.OrderBy != null)
            query = query.OrderBy(spec.OrderBy);
        else if (spec.OrderByDescending != null)
            query = query.OrderByDescending(spec.OrderByDescending);

        if (spec.IsPaginationEnabled)
            query = query.Skip(spec.Skip).Take(spec.Take);

        return query;
    }

    /// <summary>
    /// Builds and returns a query used for counting entities that match the provided specification criteria, applying filters  but excluding includes, pagination and sorting.
    /// </summary>
    /// <param name="query">The base query to apply the specification to.</param>
    /// <param name="spec">The specification containing query conditions.</param>
    /// <returns>Returns an <see cref="IQueryable{T}"/> used to count entities matching the specified criteria.</returns>
    public static IQueryable<T> GetCountQuery(IQueryable<T> query, ISpecification<T> spec)
    {
        if (spec.Criteria != null)
            query = query.Where(spec.Criteria);

        return query;
    }
}
