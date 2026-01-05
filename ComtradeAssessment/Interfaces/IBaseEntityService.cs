using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface IBaseEntityService<TEntity, TResponseDto>
{
    [OperationContract]
    Task<PagedResult<TResponseDto>> GetAll(BaseRequest request);
}
