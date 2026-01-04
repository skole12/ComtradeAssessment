using AutoMapper;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Specifications;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Services
{
    public class BaseEntityService<TEntity, TResponseDto>(
        IDatabaseContext databaseContext,
        IMapper mapper
    ) : IBaseEntityService<TEntity, TResponseDto>
        where TEntity : class
    {
        protected readonly IDatabaseContext _databaseContext = databaseContext;
        protected readonly IMapper _mapper = mapper;
        protected virtual HashSet<string> AllowedIncludes { get; } = [];

        /// <summary>
        /// Retrieves a paginated list of entities based on the provided request filters and pagination settings.
        /// </summary>
        /// <param name="request">The filtering and pagination criteria.</param>
        /// <returns>
        /// A <see cref="PagedResult{TResponseDto}"/> containing the list of mapped items and total record count.
        /// </returns>
        public virtual async Task<PagedResult<TResponseDto>> GetAll(BaseRequest request)
        {
            var spec = CreateListSpecification(request);
            var countSpec = CreateCountSpecification(request);

            var query = SpecificationEvaluator<TEntity>.GetQuery(
                _databaseContext.Set<TEntity>(),
                spec
            );
            var countQuery = SpecificationEvaluator<TEntity>.GetCountQuery(
                _databaseContext.Set<TEntity>(),
                countSpec
            );

            int totalCount = await countQuery.CountAsync();
            var items = await query.ToListAsync();

            return new PagedResult<TResponseDto>
            {
                Items = _mapper.Map<List<TResponseDto>>(items),
                Pagination = new PaginationResponse
                {
                    PageNumber = request.Pagination.PageNumber,
                    PageSize = request.Pagination.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)
                        Math.Ceiling(totalCount / (double)request.Pagination.PageSize),
                },
            };
        }

        /// <summary>
        /// Creates a specification for retrieving a filtered and paginated list of entities based on the provided request.
        /// </summary>
        /// <param name="request">The filtering and pagination criteria.</param>
        /// <returns>Returns a specification used to query entities according to the provided request parameters.</returns>
        protected virtual ISpecification<TEntity> CreateListSpecification(BaseRequest request)
        {
            return new BaseSpecification<TEntity>(request, AllowedIncludes);
        }

        /// <summary>
        /// Creates a specification for counting the total number of entities that match the provided search criteria.
        /// </summary>
        /// <param name="request">The request containing search parameters.</param>
        /// <returns>Returns a specification used to count entities matching the given filters.</returns>
        protected virtual ISpecification<TEntity> CreateCountSpecification(BaseRequest request)
        {
            var spec = new BaseSpecification<TEntity>(null);
            spec.ApplyFilters(request.Filters);

            return spec;
        }
    }
}
