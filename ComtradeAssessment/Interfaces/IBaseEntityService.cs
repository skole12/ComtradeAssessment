using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

public interface IBaseEntityService<TEntity, TResponseDto>
{
    Task<PagedResult<TResponseDto>> GetAll(BaseRequest request);
}
