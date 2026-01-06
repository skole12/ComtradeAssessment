using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface ICampaignOfferService : IBaseEntityService<CampaignOffer, CampaignOfferResponseDto>
{
    [AuthorizeByRole(ERole.SalesAgent)]
    [OperationContract]
    Task<CampaignOfferResponseDto> Create(CampaignOfferRequest request);

    [AuthorizeByRole(ERole.SalesAgent)]
    [OperationContract]
    Task Delete(CampaignOfferRequest request);

    [AuthorizeByRole(ERole.SalesAgent)]
    [OperationContract]
    new Task<PagedResult<CampaignOfferResponseDto>> GetAll(BaseRequest request);
}
