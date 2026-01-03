using ComtradeAssessment.Context;
using ComtradeAssessment.Entities;
using ComtradeAssessment.Enums;

namespace ComtradeAssessment.Workers;

public class PurchaseImportWorker
{
    private readonly IServiceScopeFactory serviceScopeFactory;

    public PurchaseImportWorker(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task ProcessCsv(string fileName, string base64conternt)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var fileBytes = Convert.FromBase64String(base64conternt);
        using var memoryStream = new MemoryStream(fileBytes);
        using var reader = new StreamReader(memoryStream);

        bool firstLine = true;
        var batchSize = 1000;
        var purchasesBatch = new List<Purchase>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            var columns = line.Split(';');
            if (columns.Length < 6)
                continue;

            var purchase = new Purchase
            {
                CampaignId = int.Parse(columns[0].Trim()),
                CustomerId = int.Parse(columns[1].Trim()),
                Date = DateTime.Parse(columns[2].Trim()),
                Amount = int.Parse(columns[3].Trim()),
                Discount = int.Parse(columns[4].Trim()),
                AmountAfterDiscount = int.Parse(columns[5].Trim()),
                PaymentType = Enum.TryParse<EPaymentType>(
                    columns.Length > 6 ? columns[6].Trim() : "Unknown",
                    out var pt
                )
                    ? pt
                    : EPaymentType.Unknown,
            };

            purchasesBatch.Add(purchase);

            if (purchasesBatch.Count >= batchSize)
            {
                await context.Purchases.AddRangeAsync(purchasesBatch);
                await context.SaveChangesAsync();
                purchasesBatch.Clear();
            }
        }

        if (purchasesBatch.Any())
        {
            await context.Purchases.AddRangeAsync(purchasesBatch);
            await context.SaveChangesAsync();
        }
    }
}
