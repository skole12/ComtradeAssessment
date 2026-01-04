using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignService
{
    [OperationContract]
    Task<CampaignResponseDto> CreateCampaign(CreateCampaignRequest request);

    [OperationContract]
    Task<CampaignResponseDto> UpdateCampaign(UpdateCampaignRequest request);

    [OperationContract]
    Task DeleteCampaign(int campaignId);

    [OperationContract]
    Task<CampaignDetailsResponseDto> CampaignDetails(int campaignId);
}
