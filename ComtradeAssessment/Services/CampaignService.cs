using System.ServiceModel;
using AutoMapper;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Enums;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Models;
using ComtradeAssessment.Workers;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Services;

public class CampaignService(
    IDatabaseContext databaseContext,
    IMapper mapper,
    ICurrentUserService currentUserService
) : BaseEntityService<Campaign, CampaignResponseDto>(databaseContext, mapper), ICampaignService
{
    private readonly ICurrentUserService currentUserService = currentUserService;

    /// <summary>
    /// Creates a new campaign based on the provided request.
    /// </summary>
    /// <param name="request">The request containing campaign details.</param>
    /// <returns>The created <see cref="CampaignResponseDto"/> representing the new campaign.</returns>
    public async Task<CampaignResponseDto> Create(CreateCampaignRequest request)
    {
        var userId = currentUserService.UserId;

        if (request.EndDate <= request.StartDate)
            throw new FaultException("EndDate must be greater than StartDate");

        var today = DateTime.Today;

        var campaign = new Campaign
        {
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = new Guid(userId),
            IsActive = today >= request.StartDate && today <= request.EndDate,
        };

        databaseContext.Campaigns.Add(campaign);
        await databaseContext.SaveChangesAsync();

        return mapper.Map<CampaignResponseDto>(campaign);
    }

    /// <summary>
    /// Retrieves the details of a campaign by its ID.
    /// </summary>
    /// <param name="campaignId">The ID of the campaign to retrieve.</param>
    /// <returns>The <see cref="CampaignDetailsResponseDto"/> containing the campaign details.</returns>
    public async Task<CampaignDetailsResponseDto> Details(int campaignId)
    {
        var result = await databaseContext
            .Campaigns.Where(c => c.Id == campaignId)
            .Select(c => new CampaignDetailsResponseDto
            {
                CampaignId = c.Id,
                Name = c.Name,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                CreatedAt = c.CreatedAt,
                CreatedBy = c.CreatedBy,
                ResultsConcluded = c.ResultsConcluded,
                IsActive = c.IsActive,
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

        return result ?? throw new FaultException("Campaign not found");
    }

    /// <summary>
    /// Deletes a campaign identified by the given campaign ID.
    /// </summary>
    public async Task Delete(int campaignId)
    {
        var campaign = await databaseContext.Campaigns.FindAsync(campaignId);

        if (campaign != null && campaign.ResultsConcluded)
            throw new FaultException("Cannot delete campaign which have concluded results");

        var deletedRows = await databaseContext
            .Campaigns.Where(c => c.Id == campaignId)
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
            throw new FaultException("Campaign not found");
    }

    /// <summary>
    /// Updates an existing campaign with the provided details.
    /// </summary>
    public async Task<CampaignResponseDto> Update(UpdateCampaignRequest request)
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
        campaign.IsActive = request.IsActive;

        await databaseContext.SaveChangesAsync();

        return mapper.Map<CampaignResponseDto>(campaign);
    }

    /// <summary>
    /// Imports purchases for a campaign based on the provided import data.
    /// </summary>
    public async Task<PurchaseImportResponse> ImportPurchases(PurchaseImportDto request)
    {
        try
        {
            var campaign =
                await databaseContext.Campaigns.FindAsync(request.CampaignId)
                ?? throw new Exception("Specified campaign does not exist");

            var trackingId = Guid.NewGuid();

            //creating temp file as to not send base64string as argument to job method
            var fileId = Guid.NewGuid();
            var bytes = Convert.FromBase64String(request.FileContentBase64);
            Directory.CreateDirectory("imports");
            var path = Path.Combine("imports", $"{fileId}.csv");
            await System.IO.File.WriteAllBytesAsync(path, bytes);

            var hangfireJobId = BackgroundJob.Enqueue<PurchaseImportWorker>(worker =>
                worker.ProcessCsv(trackingId, request.CampaignId, path)
            );

            await databaseContext.BackgroundJobStatuses.AddAsync(
                new BackgroundJobStatus
                {
                    Id = trackingId,
                    HangfireJobId = hangfireJobId,
                    Type = "PurchaseImport",
                    State = JobState.Pending,
                    CreatedAt = DateTime.UtcNow,
                }
            );

            await databaseContext.SaveChangesAsync();

            return new PurchaseImportResponse
            {
                JobId = trackingId,
                Message = "Import job successfully started",
            };
        }
        catch (Exception e)
        {
            throw new FaultException(e.Message);
        }
    }

    /// <summary>
    /// Retrieves the status of a Hangfire job by its ID.
    /// </summary>
    public async Task<CampaignJobResponseDto> JobStatus(Guid jobId)
    {
        var job = await databaseContext.BackgroundJobStatuses.FindAsync(jobId);
        if (job == null)
            throw new FaultException("Job does not exist");
        return mapper.Map<CampaignJobResponseDto>(job);
    }

    /// <summary>
    /// Downloads the file associated with the imported results for the specified campaign.
    /// </summary>
    public async Task<DownloadResultsFileResponse> DownloadResultsFile(int campaignId)
    {
        var campaign =
            await databaseContext.Campaigns.FirstOrDefaultAsync(c => c.Id == campaignId)
            ?? throw new FaultException("Campaign not found");

        if (string.IsNullOrEmpty(campaign.CsvResultsPath))
            throw new FaultException("Results file not available");

        var filePath = campaign.CsvResultsPath;

        if (!File.Exists(filePath))
            throw new FaultException("File does not exist on disk");

        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

        return new DownloadResultsFileResponse
        {
            FileName = Path.GetFileName(filePath),
            ContentType = "application/octet-stream",
            FileContentBase64 = Convert.ToBase64String(fileBytes),
        };
    }
}
