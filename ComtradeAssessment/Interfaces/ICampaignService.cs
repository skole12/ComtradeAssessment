using System.ServiceModel;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;

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
