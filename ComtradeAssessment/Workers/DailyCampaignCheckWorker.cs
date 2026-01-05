using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Workers;

public class DailyCampaignCheckWorker(IDatabaseContext databaseContext)
{
    private readonly IDatabaseContext databaseContext = databaseContext;

    public async Task Execute()
    {
        var todayUtc = DateTime.Today;

        await databaseContext
            .Campaigns.Where(c => c.IsActive && c.EndDate < todayUtc)
            .ExecuteUpdateAsync(c => c.SetProperty(c => c.IsActive, false));
    }
}
