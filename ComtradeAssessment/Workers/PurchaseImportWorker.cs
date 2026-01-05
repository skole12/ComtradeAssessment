using System.Globalization;
using System.ServiceModel;
using ComtradeAssessment.Context;
using ComtradeAssessment.Enums;
using ComtradeAssessment.Extensions;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Models;
using EFCore.BulkExtensions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ComtradeAssessment.Workers;

public class PurchaseImportWorker(
    IDatabaseContext databaseContext,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<FileStorageSettings> fileStorage
)
{
    private readonly IDatabaseContext databaseContext = databaseContext;
    private readonly IServiceScopeFactory serviceScopeFactory = serviceScopeFactory;
    private readonly IOptions<FileStorageSettings> fileStorage = fileStorage;

    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(30 * 60)]
    public async Task ProcessCsv(Guid jobId, int campaignId, string path)
    {
        var job = await databaseContext.BackgroundJobStatuses.FindAsync(jobId);
        job!.State = JobState.Processing;
        await databaseContext.SaveChangesAsync();

        try
        {
            using var scope = serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var seen = new HashSet<int>();
            var duplicates = new HashSet<int>();
            var invalidDates = new List<string>();

            const int batchSize = 3000;
            var batch = new List<(int CustomerId, DateTime PurchaseDate, string? Note)>(batchSize);

            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );

            using var reader = new StreamReader(stream);

            // skip header
            await reader.ReadLineAsync();
            var rowNumber = 1;

            while (!reader.EndOfStream)
            {
                rowNumber++;
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var columns = line.Split(';');
                if (columns.Length < 2)
                    continue;

                if (!int.TryParse(columns[0].Trim(), out var customerId))
                    throw new Exception($"Invalid customerId. Row {rowNumber}: {columns[0]}");

                if (!seen.Add(customerId))
                    duplicates.Add(customerId);

                if (
                    !DateTime.TryParseExact(
                        columns[1].Trim(),
                        "d.M.yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var purchaseDate
                    )
                )
                {
                    invalidDates.Add($"Row {rowNumber}: '{columns[1]}'");
                    continue;
                }

                var note =
                    columns.Length > 2 && !string.IsNullOrWhiteSpace(columns[2])
                        ? columns[2].Trim()
                        : null;

                batch.Add((customerId, purchaseDate, note));

                if (batch.Count >= batchSize)
                {
                    await UpdateCampaignOffersBatch(context, campaignId, batch);
                    batch.Clear();
                }
            }

            if (duplicates.Count != 0 || invalidDates.Count != 0)
            {
                var errors = new List<string>();

                if (duplicates.Count != 0)
                    errors.Add($"Duplicate customerIds: {string.Join(", ", duplicates)}");

                if (invalidDates.Count != 0)
                    errors.Add($"Invalid dates: {string.Join("; ", invalidDates)}");

                throw new Exception(string.Join(" | ", errors));
            }

            if (batch.Count != 0)
                await UpdateCampaignOffersBatch(context, campaignId, batch);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = $"campaignResults_{campaignId}_{timestamp}.csv";
            var resultPath = Path.Combine(fileStorage.Value.RootDirectory, fileName);

            Directory.CreateDirectory(fileStorage.Value.RootDirectory);

            File.Copy(path, resultPath, overwrite: true);

            await context
                .Campaigns.Where(c => c.Id == campaignId)
                .ExecuteUpdateAsync(c =>
                    c.SetProperty(c => c.ResultsConcluded, true)
                        .SetProperty(c => c.CsvResultsPath, resultPath)
                );

            job.State = JobState.Succeeded;
        }
        catch (Exception ex)
        {
            job.State = JobState.Failed;
            job.Error = ex.Message;
            throw;
        }
        finally
        {
            job.FinishedAt = DateTime.UtcNow;
            await databaseContext.SaveChangesAsync();

            // temp file cleanup
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    private async Task UpdateCampaignOffersBatch(
        DatabaseContext context,
        int campaignId,
        List<(int CustomerId, DateTime PurchaseDate, string? Note)> batch
    )
    {
        var customerIds = batch.Select(x => x.CustomerId).ToList();

        var offers = await context
            .CampaignOffers.Where(o =>
                o.CampaignId == campaignId && customerIds.Contains(o.CustomerId)
            )
            .ToListAsync();

        var lookup = batch.ToDictionary(
            x => x.CustomerId,
            x => new PurchaseCsvInfo { PurchaseDate = x.PurchaseDate, Note = x.Note }
        );

        foreach (var offer in offers)
        {
            offer.MadePurchase = true;
            var info = lookup[offer.CustomerId];
            offer.PurchaseDate = info.PurchaseDate;
            offer.Note = info.Note;
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
