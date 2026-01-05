using System.ServiceModel;
using AutoMapper;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Models;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Services;

public class CampaignOfferService(
    IDatabaseContext databaseContext,
    IMapper mapper,
    ICurrentUserService currentUserService
)
    : BaseEntityService<CampaignOffer, CampaignOfferResponseDto>(databaseContext, mapper),
        ICampaignOfferService
{
    private readonly ICurrentUserService currentUserService = currentUserService;

    /// <summary>
    /// Creates a new campaign offer role.
    /// </summary>
    /// <param name="request">The request containing campaign offer details.</param>
    /// <returns>The created <see cref="CampaignOfferResponseDto"/>.</returns>
    [AuthorizeByRole(ERole.SalesAgent)]
    public async Task<CampaignOfferResponseDto> Create(CreateCampaignOfferRequest request)
    {
        var currentDate = DateTime.UtcNow.Date;
        var userId = currentUserService.UserId;

        var campaign =
            await databaseContext.Campaigns.FindAsync(request.CampaignId)
            ?? throw new FaultException("Campaign does not exist!");

        //cannot create offer for campaigns that have concluded results or are inactive
        if (!campaign.IsActive || campaign.ResultsConcluded)
            throw new FaultException(
                "It is not possible to create offers for campaign that is not active or have concluded results!"
            );

        var numberOfAgentOffers = await databaseContext
            .CampaignOffers.Where(co =>
                co.CampaignId == request.CampaignId
                && co.AgentId == new Guid(userId)
                && co.CreatedAt.Date == currentDate
            )
            .CountAsync();

        //5 offers per day constraint
        if (numberOfAgentOffers >= 5)
            throw new FaultException("It is forbidden to create more than 5 discounts per day!");

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
            MadePurchase = campaignOffer.MadePurchase,
            PurchaseDate = campaignOffer.PurchaseDate,
            Note = campaignOffer.Note,
        };
    }

    /// <summary>
    /// Deletes a campaign offer based on the provided request.
    /// </summary>
    /// <param name="request">The request containing the ID of the campaign offer to delete.</param>
    [AuthorizeByRole(ERole.SalesAgent)]
    public async Task Delete(CreateCampaignOfferRequest request)
    {
        var campaign = await databaseContext.Campaigns.FindAsync(request.CampaignId);

        if (campaign != null && campaign.ResultsConcluded)
            throw new FaultException(
                "Cannot delete campaign offer from campaign that have concluded results"
            );

        var deletedRows = await databaseContext
            .CampaignOffers.Where(c =>
                c.CampaignId == request.CampaignId && c.CustomerId == request.CustomerId
            )
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
            throw new FaultException("Campaign offer not found");
    }

    [AuthorizeByRole(ERole.SalesAgent)]
    public override Task<PagedResult<CampaignOfferResponseDto>> GetAll(BaseRequest request)
    {
        return base.GetAll(request);
    }
}
