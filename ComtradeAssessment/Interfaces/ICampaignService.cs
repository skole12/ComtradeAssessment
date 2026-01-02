using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignService
{
    [OperationContract(Name = "CreateCampaign")]
    Task<CampaignResponseDto> CreateCampaign(CreateCampaignRequest request);
}
