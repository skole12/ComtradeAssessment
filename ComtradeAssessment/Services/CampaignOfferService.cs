using System.IdentityModel.Claims;
using System.ServiceModel;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Services;

public class CampaignOfferService : ICampaignOfferService
{
    private readonly IDatabaseContext databaseContext;
    private readonly ICurrentUser currentUser;

    public CampaignOfferService(IDatabaseContext databaseContext, ICurrentUser currentUser)
    {
        this.databaseContext = databaseContext;
        this.currentUser = currentUser;
    }

    public async Task<CampaignOfferResponseDto> CreateCampaignOffer(
        CreateCampaignOfferRequest request
    )
    {
        var userId = currentUser.UserId;

        var numberOfAgentOffers = await databaseContext
            .CampaignOffers.Where(co =>
                co.CampaignId == request.CampaignId
                && co.AgentId == new Guid(userId)
                && co.CreatedAt.Date == DateTime.UtcNow.Date
            )
            .CountAsync();

        if (numberOfAgentOffers >= 5)
        {
            throw new FaultException("It is forbidden to create more than 5 discounts per day!");
        }

        var campaignOffer = new CampaignOffer
        {
            CampaignId = request.CampaignId,
            AgentId = new Guid(userId),
            CustomerId = request.CustomerId,
            CreatedAt = DateTime.UtcNow,
        };

        databaseContext.CampaignOffers.Add(campaignOffer);
        await databaseContext.SaveChangesAsync();

        return new CampaignOfferResponseDto
        {
            CampaignId = campaignOffer.CampaignId,
            AgentId = campaignOffer.AgentId,
            CustomerId = campaignOffer.CustomerId,
            CreatedAt = campaignOffer.CreatedAt,
        };
    }
}
