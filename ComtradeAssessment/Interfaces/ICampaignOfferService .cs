using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
[AuthorizeServiceByRole(ERole.SalesAgent)]
public interface ICampaignOfferService : IBaseEntityService<CampaignOffer, CampaignOfferResponseDto>
{
    [OperationContract]
    Task<CampaignOfferResponseDto> Create(CampaignOfferRequest request);

    [OperationContract]
    Task Delete(CampaignOfferRequest request);

    [OperationContract]
    new Task<PagedResult<CampaignOfferResponseDto>> GetAll(BaseRequest request);
}
