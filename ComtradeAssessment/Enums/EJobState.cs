namespace ComtradeAssessment.Enums;

/// <summary>
/// Represents the current state of a Hangfire job.
/// </summary>
public enum JobState
{
    Pending,
    Processing,
    Succeeded,
    Failed,
}
