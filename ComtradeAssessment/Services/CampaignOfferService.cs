using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Specifications;
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

    [AuthorizeByRole(ERole.SalesAgent)]
    public async Task<CampaignOfferResponseDto> Create(CreateCampaignOfferRequest request)
    {
        var currentDate = DateTime.UtcNow.Date;
        var userId = currentUser.UserId;

        var campaign =
            await databaseContext.Campaigns.FindAsync(request.CampaignId)
            ?? throw new FaultException("Campaign does not exist!");

        if (currentDate > campaign.EndDate.Date || campaign.ResultsConcluded)
            throw new FaultException("It is forbidden to create offers for campaign that ended!");

        var numberOfAgentOffers = await databaseContext
            .CampaignOffers.Where(co =>
                co.CampaignId == request.CampaignId
                && co.AgentId == new Guid(userId)
                && co.CreatedAt.Date == currentDate
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
            MadePurchase = campaignOffer.MadePurchase,
            PurchaseDate = campaignOffer.PurchaseDate,
            Note = campaignOffer.Note,
        };
    }

    [AuthorizeByRole(ERole.SalesAgent)]
    public async Task Delete(DeleteCampaignOfferRequest request)
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

    public async Task<PagedResult<CampaignOfferResponseDto>> GetAll(BaseRequest request)
    {
        var spec = CreateListSpecification(request);
        var countSpec = CreateCountSpecification(request);

        var query = SpecificationEvaluator<CampaignOffer>.GetQuery(
            databaseContext.Set<CampaignOffer>(),
            spec
        );
        var countQuery = SpecificationEvaluator<CampaignOffer>.GetCountQuery(
            databaseContext.Set<CampaignOffer>(),
            countSpec
        );

        int totalCount = await countQuery.CountAsync();
        var items = await query.ToListAsync();

        return new PagedResult<CampaignOfferResponseDto>
        {
            Items =
            [
                .. items.Select(co => new CampaignOfferResponseDto
                {
                    CustomerId = co.CustomerId,
                    CampaignId = co.CampaignId,
                    CreatedAt = co.CreatedAt,
                    MadePurchase = co.MadePurchase,
                    PurchaseDate = co.PurchaseDate,
                    Note = co.Note,
                }),
            ],
            Pagination = new PaginationResponse
            {
                PageNumber = request.Pagination.PageNumber,
                PageSize = request.Pagination.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.Pagination.PageSize),
            },
        };
    }

    protected virtual HashSet<string> AllowedIncludes { get; } = [];

    /// <summary>
    /// Creates a specification for retrieving a filtered and paginated list of entities based on the provided request.
    /// </summary>
    /// <param name="request">The filtering and pagination criteria.</param>
    /// <returns>Returns a specification used to query entities according to the provided request parameters.</returns>
    protected virtual ISpecification<CampaignOffer> CreateListSpecification(BaseRequest request)
    {
        return new BaseSpecification<CampaignOffer>(request, AllowedIncludes);
    }

    /// <summary>
    /// Creates a specification for counting the total number of entities that match the provided search criteria.
    /// </summary>
    /// <param name="request">The request containing search parameters.</param>
    /// <returns>Returns a specification used to count entities matching the given filters.</returns>
    protected virtual ISpecification<CampaignOffer> CreateCountSpecification(BaseRequest request)
    {
        var spec = new BaseSpecification<CampaignOffer>(null);
        spec.ApplyFilters(request.Filters);

        return spec;
    }
}
