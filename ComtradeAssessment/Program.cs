using ComtradeAssessment.Extensions;
using ComtradeAssessment.Middlewares;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();
app.UseHttpsRedirection();
app.UseHangfireDashboard("/hangfire");

app.UseMiddleware<AuthMiddleware>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapSoapEndpoints();
});

app.Run();
