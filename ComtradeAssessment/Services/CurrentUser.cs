using System.ServiceModel;
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
                throw new FaultException("UserId not found in context");

            return userId;
        }
    }
}
