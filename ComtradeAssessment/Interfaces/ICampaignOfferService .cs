using System.ServiceModel;
using ComtradeAssessment.DTO;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignOfferService
{
    [OperationContract]
    Task<CampaignOfferResponseDto> CreateCampaignOffer(CreateCampaignOfferRequest request);

    [OperationContract]
    Task DeleteCampaignOffer(DeleteCampaignOfferRequest request);
}
