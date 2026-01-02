using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;

namespace ComtradeAssessment.Services;

public class CampaignService : ICampaignService
{
    private readonly IDatabaseContext databaseContext;

    public CampaignService(IDatabaseContext databaseContext)
    {
        this.databaseContext = databaseContext;
    }

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task<CampaignResponseDto> CreateCampaign(CreateCampaignRequest request)
    {
        if (request.EndDate <= request.StartDate)
        {
            throw new ArgumentException("EndDate must be greater than StartDate");
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var campaign = new Campaign
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        };

        databaseContext.Campaigns.Add(campaign);
        await databaseContext.SaveChangesAsync();

        return new CampaignResponseDto
        {
            Id = campaign.Id,
            Name = campaign.Name,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
        };
    }
}
