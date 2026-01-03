using ComtradeAssessment.Interfaces;

namespace ComtradeAssessment.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();

            if (string.IsNullOrWhiteSpace(userId))
                throw new UnauthorizedAccessException("UserId not found in context");

            return userId;
        }
    }
}
