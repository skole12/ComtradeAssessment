using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignService
{
    [OperationContract]
    Task<CampaignResponseDto> Create(CreateCampaignRequest request);

    [OperationContract]
    Task<CampaignResponseDto> Update(UpdateCampaignRequest request);

    [OperationContract]
    Task Delete(int campaignId);

    [OperationContract]
    Task<CampaignDetailsResponseDto> Details(int campaignId);

    [OperationContract]
    Task<PagedResult<CampaignResponseDto>> GetAll(BaseRequest request);
}
