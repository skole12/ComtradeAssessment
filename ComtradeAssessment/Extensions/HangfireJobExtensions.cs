using ComtradeAssessment.Constants;
using ComtradeAssessment.Workers;
using Hangfire;

namespace ComtradeAssessment.Extensions;

public static class HangfireJobExtensions
{
    /// <summary>
    /// Registers recurring Hangfire jobs for the application.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder"/> used to configure the jobs.</param>
    public static void RegisterRecurringJobs(this IApplicationBuilder app)
    {
        RecurringJob.AddOrUpdate<DailyCampaignCheckWorker>(
            "daily-campaign-check",
            worker => worker.Execute(),
            "10 0 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(ETimeZone.CentralEuropean),
            }
        );
    }
}
