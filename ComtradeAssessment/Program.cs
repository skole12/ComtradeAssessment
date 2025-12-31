using ComtradeAssessment.Extensions;
using SoapCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddSoapCore();

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.Run();
