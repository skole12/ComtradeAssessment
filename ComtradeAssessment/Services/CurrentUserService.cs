using System.ServiceModel;
using ComtradeAssessment.Interfaces;

namespace ComtradeAssessment.Services;

/// <summary>
/// Provides information about the currently authenticated user from the HTTP context.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Gets the ID of the currently authenticated user.
    /// </summary>
    /// <exception cref="FaultException">Thrown if the UserId is not found in the HTTP context.</exception>
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
