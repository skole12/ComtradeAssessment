namespace ComtradeAssessment.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class AuthorizeServiceByRoleAttribute : Attribute
{
    public string[] Roles { get; }

    public AuthorizeServiceByRoleAttribute(params string[] roles)
    {
        Roles = roles?.Select(r => r.ToString()).ToArray() ?? Array.Empty<string>();
    }
}
