using System.ServiceModel;
using ComtradeAssessment.Interfaces;

namespace ComtradeAssessment.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

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
