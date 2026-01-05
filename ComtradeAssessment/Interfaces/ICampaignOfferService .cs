using System.ServiceModel;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignOfferService : IBaseEntityService<CampaignOffer, CampaignOfferResponseDto>
{
    [OperationContract]
    Task<CampaignOfferResponseDto> Create(CreateCampaignOfferRequest request);

    [OperationContract]
    Task Delete(DeleteCampaignOfferRequest request);
}
