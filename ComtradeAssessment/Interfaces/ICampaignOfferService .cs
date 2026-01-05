using System.ServiceModel;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignOfferService : IBaseEntityService<CampaignOffer, CampaignOfferResponseDto>
{
    [OperationContract]
    Task<CampaignOfferResponseDto> Create(CampaignOfferRequest request);

    [OperationContract]
    Task Delete(CampaignOfferRequest request);
}
