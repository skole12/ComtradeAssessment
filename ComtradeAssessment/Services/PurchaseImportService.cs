using ComtradeAssessment.DTO;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Workers;
using Hangfire;

namespace ComtradeAssessment.Services;

public class PurchaseImportService : IPurchaseImportService
{
    public PurchaseImportResponse ImportPurchases(PurchaseImportDto request)
    {
        try
        {
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
            return new PurchaseImportResponse
            {
                Success = false,
                Message = "Error creating background job: " + ex.Message,
            };
        }
    }
}
