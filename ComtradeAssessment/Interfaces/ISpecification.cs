using System.Linq.Expressions;

namespace ComtradeAssessment.Interfaces;

public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    Expression<Func<T, object>>? OrderBy { get; }
    Expression<Func<T, object>>? OrderByDescending { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    HashSet<string>? AllowedIncludes { get; }
    List<string> IncludeStrings { get; }
    int Take { get; }
    int Skip { get; }
    bool IsPaginationEnabled { get; }
}

//TODO delete this
public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>>? Select { get; }
}
