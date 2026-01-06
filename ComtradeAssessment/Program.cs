using ComtradeAssessment.Cache;
using ComtradeAssessment.Extensions;
using ComtradeAssessment.Middlewares;
using ComtradeAssessment.Seeding;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
AuthCacheInitializer.BuildAuthCache();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
await Seed.SeedData(app.Services);
app.UseRouting();
app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire");
app.RegisterRecurringJobs();
app.UseMiddleware<AuthMiddleware>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSoapEndpoints();
});

app.Run();
