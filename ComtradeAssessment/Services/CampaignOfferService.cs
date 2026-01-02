using System.IdentityModel.Claims;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;

namespace ComtradeAssessment.Services;

public class CampaignOfferService : ICampaignOfferService
{
    private readonly IDatabaseContext databaseContext;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CampaignOfferService(
        IDatabaseContext databaseContext,
        IHttpContextAccessor httpContextAccessor
    )
    {
        this.databaseContext = databaseContext;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<CampaignOfferResponseDto> CreateCampaignOffer(
        CreateCampaignOfferRequest request
    )
    {
        var userId =
            httpContextAccessor.HttpContext?.Items["UserId"]?.ToString()
            ?? throw new UnauthorizedAccessException();

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
            Id = campaignOffer.Id,
            CampaignId = campaignOffer.CampaignId,
            AgentId = campaignOffer.AgentId,
            CustomerId = campaignOffer.CustomerId,
            CreatedAt = campaignOffer.CreatedAt,
        };
    }
}
