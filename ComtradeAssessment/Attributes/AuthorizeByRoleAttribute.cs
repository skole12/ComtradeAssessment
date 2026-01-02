using ComtradeAssessment.Entities;

namespace ComtradeAssessment.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class AuthorizeByRoleAttribute : Attribute
{
    public string[] Roles { get; }

    public AuthorizeByRoleAttribute(params string[] roles)
    {
        Roles = roles.Select(r => r.ToString()).ToArray();
    }
}
