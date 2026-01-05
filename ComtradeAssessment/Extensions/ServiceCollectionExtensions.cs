using ComtradeAssessment.Context;
using ComtradeAssessment.Interfaces;
using ComtradeAssessment.Mappers;
using ComtradeAssessment.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using SoapCore;

namespace ComtradeAssessment.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application-level services into the dependency injection container.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the application services will be added.
    /// </param>
    /// <param name="config">
    /// The <see cref="IConfiguration"/> instance used to access application settings,
    /// e.g., for configuring options or seeding initial data.
    /// </param>
    /// <returns>
    /// The updated <see cref="IServiceCollection"/> with all application services registered.
    /// </returns>
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

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICampaignService, CampaignService>();
        services.AddScoped<ICampaignOfferService, CampaignOfferService>();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<Mapper>();
        });
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
