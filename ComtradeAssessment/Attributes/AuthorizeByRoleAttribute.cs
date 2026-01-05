namespace ComtradeAssessment.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AuthorizeByRoleAttribute(params string[] roles) : Attribute
{
    public string[] Roles { get; } = [.. roles.Select(r => r.ToString())];
}
