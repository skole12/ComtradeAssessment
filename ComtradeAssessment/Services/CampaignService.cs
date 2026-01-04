using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            throw new FaultException("EndDate must be greater than StartDate");
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

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task<CampaignResultsResponseDto> CampaignResults(GetCampaignResults request)
    {
        var results = await databaseContext
            .CampaignOffers.Where(co => co.CampaignId == request.CampaignId)
            .GroupBy(co => co.CampaignId)
            .Select(g => new { Total = g.Count(), Used = g.Count(co => co.MadePurchase) })
            .FirstOrDefaultAsync();

        if (results == null)
        {
            throw new FaultException("There are no results for given campaign");
        }

        return new CampaignResultsResponseDto
        {
            CampaignId = request.CampaignId,
            DiscountsOffered = results.Total,
            PurchasesMade = results.Used,
            SuccessRate = results.Total > 0 ? (float)results.Used / results.Total * 100 : 0,
        };
    }
}
