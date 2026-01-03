using System.Globalization;
using System.Text.RegularExpressions;
using ComtradeAssessment.Context;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Workers;

public class PurchaseImportWorker(IServiceScopeFactory serviceScopeFactory)
{
    private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;

    public async Task ProcessCsv(int campaignId, string base64Content)
    {
        using var scope = serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        var fileBytes = Convert.FromBase64String(base64Content);

        var seen = new HashSet<int>();
        var duplicates = new HashSet<int>();
        var invalidDates = new List<string>();

        using (var validationStream = new MemoryStream(fileBytes))
        using (var reader = new StreamReader(validationStream))
        {
            // skip header
            reader.ReadLine();
            var rowNumber = 1;

            while (!reader.EndOfStream)
            {
                rowNumber++;
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(';');
                if (columns.Length < 2)
                    continue;

                if (!int.TryParse(columns[0].Trim(), out var customerId))
                    throw new Exception($"Invalid customerId: {columns[0]}");

                if (!seen.Add(customerId))
                {
                    duplicates.Add(customerId);
                }

                if (
                    !DateTime.TryParseExact(
                        columns[1].Trim(),
                        "d.M.yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out _
                    )
                )
                {
                    invalidDates.Add($"Red {rowNumber}: '{columns[1]}'");
                }
            }
        }
        if (duplicates.Any() || invalidDates.Any())
        {
            var errors = new List<string>();

            if (duplicates.Any())
                errors.Add($"Double customerIds: {string.Join(", ", duplicates)}");

            if (invalidDates.Any())
                errors.Add($"Invalid dates: {string.Join("; ", invalidDates)}");

            throw new Exception(string.Join(" | ", errors));
        }

        using var processingStream = new MemoryStream(fileBytes);
        using var readerProcessing = new StreamReader(processingStream);

        var batchSize = 3000;
        var batch = new List<(int CustomerId, DateTime PurchaseDate)>(batchSize);

        // skip header
        readerProcessing.ReadLine();

        while (!readerProcessing.EndOfStream)
        {
            var line = readerProcessing.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var columns = line.Split(';');
            if (columns.Length < 2)
                continue;

            batch.Add(
                (
                    CustomerId: int.Parse(columns[0].Trim()),
                    PurchaseDate: DateTime.Parse(columns[1].Trim())
                )
            );

            if (batch.Count >= batchSize)
            {
                await UpdateCampaignOffersBatch(context, campaignId, batch);
                batch.Clear();
            }
        }

        if (batch.Any())
        {
            await UpdateCampaignOffersBatch(context, campaignId, batch);
        }

        await context
            .Campaigns.Where(c => c.Id == campaignId)
            .ExecuteUpdateAsync(c => c.SetProperty(c => c.ResultsConcluded, true));
    }

    private async Task UpdateCampaignOffersBatch(
        DatabaseContext context,
        int campaignId,
        List<(int CustomerId, DateTime PurchaseDate)> batch
    )
    {
        var customerIds = batch.Select(x => x.CustomerId).ToList();

        var offers = await context
            .CampaignOffers.Where(o =>
                o.CampaignId == campaignId && customerIds.Contains(o.CustomerId)
            )
            .ToListAsync();

        var lookup = batch.ToDictionary(x => x.CustomerId, x => x.PurchaseDate);

        foreach (var offer in offers)
        {
            offer.MadePurchase = true;
            offer.PurchaseDate = lookup[offer.CustomerId];
        }

        await context.BulkUpdateAsync(
            offers,
            new BulkConfig
            {
                BatchSize = 3000,
                PreserveInsertOrder = false,
                SetOutputIdentity = false,
            }
        );
    }
}
