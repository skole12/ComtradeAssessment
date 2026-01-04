using System.Linq.Expressions;
using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.DTO;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Specifications;
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
            ResultsConcluded = campaign.ResultsConcluded,
        };
    }

    public async Task<GetAllCampaignsResponse> GetAll(BaseRequest request)
    {
        var spec = CreateListSpecification(request);
        var countSpec = CreateCountSpecification(request);

        var query = SpecificationEvaluator<Campaign>.GetQuery(
            databaseContext.Set<Campaign>(),
            spec
        );
        var countQuery = SpecificationEvaluator<Campaign>.GetCountQuery(
            databaseContext.Set<Campaign>(),
            countSpec
        );

        int totalCount = await countQuery.CountAsync();
        var items = await query.ToListAsync();

        return new GetAllCampaignsResponse
        {
            Items = items
                .Select(c => new CampaignResponseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    ResultsConcluded = c.ResultsConcluded,
                })
                .ToList(),
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
    protected virtual ISpecification<Campaign> CreateListSpecification(BaseRequest request)
    {
        return new BaseSpecification<Campaign>(request, AllowedIncludes);
    }

    /// <summary>
    /// Creates a specification for counting the total number of entities that match the provided search criteria.
    /// </summary>
    /// <param name="request">The request containing search parameters.</param>
    /// <returns>Returns a specification used to count entities matching the given filters.</returns>
    protected virtual ISpecification<Campaign> CreateCountSpecification(BaseRequest request)
    {
        var spec = new BaseSpecification<Campaign>(null);
        spec.ApplyFilters(request.Filters);

        return spec;
    }
}
