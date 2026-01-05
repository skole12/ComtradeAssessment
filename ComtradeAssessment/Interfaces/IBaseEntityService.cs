using System.ServiceModel;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface IBaseEntityService<TEntity, TResponseDto>
{
    [OperationContract]
    Task<PagedResult<TResponseDto>> GetAll(BaseRequest request);

    //we can implement generic create/add/update  but its not needed for current implementation
}
