using System.ServiceModel;
using ComtradeAssessment.Attributes;
using ComtradeAssessment.Constants;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Models;
using ComtradeAssessment.Workers;
using Hangfire;

namespace ComtradeAssessment.Services;

public class PurchaseImportService(IDatabaseContext databaseContext) : IPurchaseImportService
{
    private readonly IDatabaseContext databaseContext = databaseContext;

    [AuthorizeByRole(ERole.SalesManager)]
    public async Task<PurchaseImportResponse> ImportPurchases(PurchaseImportDto request)
    {
        try
        {
            var campaign =
                await databaseContext.Campaigns.FindAsync(request.CampaignId)
                ?? throw new FaultException("Specified campaign does not exist");

            if (campaign.ResultsConcluded)
                throw new FaultException("Results for this campaign are already imported.");

            BackgroundJob.Enqueue<PurchaseImportWorker>(worker =>
                worker.ProcessCsv(request.CampaignId, request.FileContentBase64)
            );
            return new PurchaseImportResponse
            {
                Success = true,
                Message = "Import job successfully started",
            };
        }
        catch (Exception ex)
        {
            return new PurchaseImportResponse { Success = false, Message = ex.Message };
        }
    }
}
