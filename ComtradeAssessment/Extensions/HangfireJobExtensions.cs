using ComtradeAssessment.Workers;
using Hangfire;

namespace ComtradeAssessment.Extensions;

public static class HangfireJobExtensions
{
    public static void RegisterRecurringJobs(this IApplicationBuilder app)
    {
        RecurringJob.AddOrUpdate<DailyCampaignCheckWorker>(
            "daily-campaign-check",
            worker => worker.Execute(),
            "10 0 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"),
            }
        );
    }
}
