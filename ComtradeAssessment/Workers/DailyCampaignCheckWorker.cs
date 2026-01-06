using ComtradeAssessment.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComtradeAssessment.Workers;

public class DailyCampaignCheckWorker(IDatabaseContext databaseContext)
{
    private readonly IDatabaseContext databaseContext = databaseContext;

    public async Task Execute()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        await databaseContext
            .Campaigns.Where(c => c.IsActive && c.EndDate < today)
            .ExecuteUpdateAsync(c => c.SetProperty(c => c.IsActive, false));

        await databaseContext
            .Campaigns.Where(c => !c.ResultsConcluded && !c.IsActive && c.StartDate.Date <= today)
            .ExecuteUpdateAsync(c => c.SetProperty(c => c.IsActive, true));
    }
}
