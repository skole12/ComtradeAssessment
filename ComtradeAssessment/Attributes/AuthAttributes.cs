namespace ComtradeAssessment.Attributes;

/// <summary>
/// Marks a service method as publicly accessible and skips any authorization checks.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AllowAnonymousAttribute : Attribute { }

/// <summary>
/// Defines role-based authorization for a specific service method.
/// This attribute has higher priority than service-level authorization.
/// </summary>
/// <param name="roles">Allowed roles for accessing the method.</param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class AuthorizeByRoleAttribute(params string[] roles) : Attribute
{
    public string[] Roles { get; } = [.. roles.Select(r => r.ToString())];
}

/// <summary>
/// Defines role-based authorization for the entire service interface.
/// Applies to all methods unless overridden at method level.
/// </summary>
/// <param name="roles">Allowed roles for accessing the service.</param>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class AuthorizeServiceByRoleAttribute(params string[] roles) : Attribute
{
    public string[] Roles { get; } = roles?.Select(r => r.ToString()).ToArray() ?? [];
}
