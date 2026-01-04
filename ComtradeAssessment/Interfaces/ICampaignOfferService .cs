using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignOfferService
{
    [OperationContract]
    Task<CampaignOfferResponseDto> Create(CreateCampaignOfferRequest request);

    [OperationContract]
    Task Delete(DeleteCampaignOfferRequest request);

    [OperationContract]
    Task<PagedResult<CampaignOfferResponseDto>> GetAll(BaseRequest request);
}
