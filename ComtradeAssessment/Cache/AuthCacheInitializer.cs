using System.Reflection;
using System.ServiceModel;
using ComtradeAssessment.Attributes;

namespace ComtradeAssessment.Cache;

public static class AuthCacheInitializer
{
    //putting service/methods auth annotations in cache at startup so we avoid reflection every request
    public static void BuildAuthCache()
    {
        var allServices = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetCustomAttribute<ServiceContractAttribute>() != null);

        foreach (var serviceType in allServices)
        {
            var serviceAttr = serviceType.GetCustomAttribute<AuthorizeServiceByRoleAttribute>();

            var entry = new AuthRuleCacheEntry
            {
                ServiceType = serviceType,
                OperationRules = serviceType
                    .GetMethods()
                    .Where(m => m.GetCustomAttribute<OperationContractAttribute>() != null)
                    .ToDictionary(
                        m => m.Name,
                        m =>
                        {
                            // service level auth
                            if (serviceAttr != null)
                                return new AuthRule
                                {
                                    RequiresAuth = true,
                                    Roles = serviceAttr.Roles,
                                };

                            // method level auth
                            var methodAuth = m.GetCustomAttribute<AuthorizeByRoleAttribute>();
                            if (methodAuth != null)
                                return new AuthRule
                                {
                                    RequiresAuth = true,
                                    Roles = methodAuth.Roles,
                                };

                            // allow anonymous
                            if (m.GetCustomAttribute<AllowAnonymousAttribute>() != null)
                                return new AuthRule { RequiresAuth = false };

                            // default auth on
                            return new AuthRule { RequiresAuth = true };
                        }
                    ),
            };

            AuthCache.ServiceAuthCache.TryAdd(serviceType.Name, entry);
        }
    }
}
