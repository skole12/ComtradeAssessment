using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignService
{
    [OperationContract]
    Task<CampaignResponseDto> CreateCampaign(CreateCampaignRequest request);

    [OperationContract]
    Task<CampaignResultsResponseDto> CampaignResults(GetCampaignResults request);
}
