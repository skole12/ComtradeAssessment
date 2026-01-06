using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ComtradeAssessment.Cache;
using ComtradeAssessment.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ComtradeAssessment.Middlewares;

public sealed class AuthMiddleware(RequestDelegate next, IOptions<JwtSettings> options)
{
    private readonly RequestDelegate _next = next;
    private readonly JwtSettings _jwtSettings = options.Value;

    private const string WsseNamespace =
        "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

    public async Task InvokeAsync(HttpContext context)
    {
        if (
            !context.Request.Path.Value?.EndsWith(".svc", StringComparison.OrdinalIgnoreCase)
            ?? true
        )
        {
            await _next(context);
            return;
        }

        //not blocking ?wsdl requests
        if (
            context.Request.QueryString.HasValue
            && context.Request.QueryString.Value.Contains(
                "wsdl",
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();
        var body = await ReadRequestBodyAsync(context.Request);

        try
        {
            var doc = XDocument.Parse(body);
            var operation = GetSoapOperationName(doc);

            if (string.IsNullOrWhiteSpace(operation))
            {
                await _next(context);
                return;
            }

            var serviceType = ResolveServiceType(context);
            if (serviceType == null)
            {
                await WriteSoapFault(context, "Unknown service");
                return;
            }

            var rule = ResolveAuthRule(serviceType, operation);

            if (!rule.RequiresAuth)
            {
                await _next(context);
                return;
            }

            var principal = ValidateBearerToken(doc, context);
            context.User = principal;

            if (rule.Roles.Length > 0)
            {
                var userRoles = principal.FindAll(ClaimTypes.Role).Select(r => r.Value);
                if (!rule.Roles.Any(r => userRoles.Contains(r)))
                {
                    await WriteSoapFault(context, "Forbidden");
                    return;
                }
            }

            await _next(context);
        }
        catch (SecurityTokenException ex)
        {
            await WriteSoapFault(context, ex.Message);
        }
        catch (XmlException)
        {
            await WriteSoapFault(context, "Invalid SOAP request format");
        }
        catch (Exception)
        {
            await WriteSoapFault(context, "Authentication failed");
        }
    }

    private static AuthRule ResolveAuthRule(Type serviceType, string operation)
    {
        if (!AuthCache.ServiceAuthCache.TryGetValue(serviceType.Name, out var entry))
            return new AuthRule { RequiresAuth = true };

        if (entry.OperationRules.TryGetValue(operation, out var rule))
            return rule;

        return new AuthRule { RequiresAuth = true };
    }

    private static Type? ResolveServiceType(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (string.IsNullOrWhiteSpace(path))
            return null;

        var serviceName = Path.GetFileNameWithoutExtension(path);

        var fullServiceName = $"I{serviceName}Service";

        if (AuthCache.ServiceAuthCache.TryGetValue(fullServiceName, out var entry))
        {
            return entry.ServiceType;
        }

        return null;
    }

    private ClaimsPrincipal ValidateBearerToken(XDocument doc, HttpContext context)
    {
        XNamespace wsse = WsseNamespace;

        var token = doc.Descendants(wsse + "BinarySecurityToken")
            .FirstOrDefault()
            ?.Value?.Replace("\r", "")
            .Replace("\n", "")
            .Trim();

        if (string.IsNullOrWhiteSpace(token))
            throw new SecurityTokenException("Bearer token missing");

        var handler = new JwtSecurityTokenHandler();

        var principal = handler.ValidateToken(
            token,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_jwtSettings.Key)
                ),
                ClockSkew = TimeSpan.Zero,
            },
            out _
        );

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        context.Items["UserId"] = userId;

        return principal;
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;
        return body;
    }

    private static string? GetSoapOperationName(XDocument doc)
    {
        XNamespace soap = "http://schemas.xmlsoap.org/soap/envelope/";
        return doc.Descendants(soap + "Body").Elements().FirstOrDefault()?.Name.LocalName;
    }

    private static async Task WriteSoapFault(HttpContext context, string message)
    {
        context.Response.StatusCode = 401;
        context.Response.ContentType = "text/xml; charset=utf-8";

        var fault = $"""
<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">
  <s:Body>
    <s:Fault>
      <faultcode>s:Client</faultcode>
      <faultstring>{System.Security.SecurityElement.Escape(message)}</faultstring>
    </s:Fault>
  </s:Body>
</s:Envelope>
""";

        await context.Response.WriteAsync(fault);
    }
}
