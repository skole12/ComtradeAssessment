using ComtradeAssessment.Enums;

namespace ComtradeAssessment.Entities;

public class BackgroundJobStatus
{
    public Guid Id { get; set; }
    public string HangfireJobId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public JobState State { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
}
