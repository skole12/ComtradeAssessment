using System.ServiceModel;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Models;

namespace ComtradeAssessment.Interfaces;

public interface ICampaignService : IBaseEntityService<Campaign, CampaignResponseDto>
{
    [OperationContract]
    Task<CampaignResponseDto> Create(CreateCampaignRequest request);

    [OperationContract]
    Task<CampaignResponseDto> Update(UpdateCampaignRequest request);

    [OperationContract]
    Task Delete(int campaignId);

    [OperationContract]
    Task<CampaignDetailsResponseDto> Details(int campaignId);
}
