using ComtradeAssessment.Context;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using SoapCore;

namespace ComtradeAssessment.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddDbContext<DatabaseContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"))
        );
        services.AddScoped<IDatabaseContext>(provider =>
            provider.GetRequiredService<DatabaseContext>()
        );
        services.AddHttpContextAccessor();
        services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
        services.Configure<FileStorageSettings>(config.GetSection("FileStorage"));

        services.AddHangfire(options =>
            options
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(config.GetConnectionString("DefaultConnection"))
        );

        services.AddHangfireServer();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<ICampaignOfferService, CampaignOfferService>();
        services.AddScoped<IPurchaseImportService, PurchaseImportService>();

        services.AddSoapCore();

        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                builder =>
                {
                    builder
                        .WithOrigins("http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }
            );
        });

        return services;
    }
}
