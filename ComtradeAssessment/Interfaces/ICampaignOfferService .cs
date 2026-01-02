using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignOfferService
{
    [OperationContract(Name = "CreateCampaignOffer")]
    Task<CampaignOfferResponseDto> CreateCampaignOffer(CreateCampaignOfferRequest request);
}
