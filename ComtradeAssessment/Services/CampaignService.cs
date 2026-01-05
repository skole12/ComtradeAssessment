using System.ServiceModel;
using AutoMapper;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Enums;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Models;
using ComtradeAssessment.Workers;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Services;

public class CampaignService(IDatabaseContext databaseContext, IMapper mapper)
    : BaseEntityService<Campaign, CampaignResponseDto>(databaseContext, mapper),
        ICampaignService
{
    [AuthorizeByRole(ERole.SalesManager)]
    public async Task<CampaignResponseDto> Create(CreateCampaignRequest request)
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
            IsActive = true,
        };

        databaseContext.Campaigns.Add(campaign);
        await databaseContext.SaveChangesAsync();

        return mapper.Map<CampaignResponseDto>(campaign);
    }

    [AuthorizeByRole(ERole.SalesManager)]
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

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task Delete(int campaignId)
    {
        var deletedRows = await databaseContext
            .Campaigns.Where(c => c.Id == campaignId)
            .ExecuteDeleteAsync();

        if (deletedRows == 0)
            throw new FaultException("Campaign not found");
    }

    [AuthorizeByRole(ERole.SalesManager)]
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

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task<PurchaseImportResponse> ImportPurchases(PurchaseImportDto request)
    {
        try
        {
            var campaign =
                await databaseContext.Campaigns.FindAsync(request.CampaignId)
                ?? throw new Exception("Specified campaign does not exist");

            if (campaign.ResultsConcluded)
                throw new Exception("Results for this campaign are already imported.");

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

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task<CampaignJobResponseDto> JobStatus(Guid jobId)
    {
        var job = await databaseContext.BackgroundJobStatuses.FindAsync(jobId);
        if (job == null)
            throw new FaultException("Job does not exist");
        return mapper.Map<CampaignJobResponseDto>(job);
    }

    [AuthorizeByRole(ERole.SalesManager)]
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
