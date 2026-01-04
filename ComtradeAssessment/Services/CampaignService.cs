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
    public async Task<CampaignDetailsResponseDto> CampaignDetails(int campaignId)
    {
        var result = await databaseContext
            .Campaigns.Where(c => c.Id == campaignId)
            .Select(c => new CampaignDetailsResponseDto
            {
                CampaignId = c.Id,
                Name = c.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                ResultsConcluded = c.ResultsConcluded,
                DiscountsOffered = c.CampaignOffers.Count(),
                PurchasesMade = c.CampaignOffers.Count(co => co.MadePurchase),
                SuccessRate =
                    c.CampaignOffers.Count() > 0
                        ? (float)c.CampaignOffers.Count(co => co.MadePurchase)
                            / c.CampaignOffers.Count()
                            * 100
                        : 0,
            })
            .FirstOrDefaultAsync();

        if (result == null)
            throw new FaultException("Campaign not found");

        return result;
    }

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task DeleteCampaign(int campaignId)
    {
        var deletedRows = await databaseContext
            .Campaigns.Where(c => c.Id == campaignId)
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
            throw new FaultException("Campaign not found");
    }

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task<CampaignResponseDto> UpdateCampaign(UpdateCampaignRequest request)
    {
        var campaign = await databaseContext.Campaigns.FindAsync(request.Id);
        if (campaign == null)
            throw new FaultException("Campaign not found");

        if (campaign.ResultsConcluded)
            throw new FaultException("Cannot update campaign which have concluded results");

        if (request.EndDate <= request.StartDate)
        {
            throw new FaultException("EndDate must be greater than StartDate");
        }

        campaign.Name = request.Name;
        campaign.StartDate = request.StartDate;
        campaign.EndDate = request.EndDate;

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
