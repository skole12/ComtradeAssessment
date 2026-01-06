using System.Collections.Concurrent;

namespace ComtradeAssessment.Cache;

public static class AuthCache
{
    //global cache for all servies and methods
    public static readonly ConcurrentDictionary<string, AuthRuleCacheEntry> ServiceAuthCache = new(
        StringComparer.OrdinalIgnoreCase
    );
}

public sealed class AuthRuleCacheEntry
{
    public Type ServiceType { get; init; } = null!;
    public Dictionary<string, AuthRule> OperationRules { get; init; } = [];
}

public sealed class AuthRule
{
    public bool RequiresAuth { get; init; }
    public string[] Roles { get; init; } = [];
}
