using CfcTicketWatcher.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Register HttpClient for ticket API calls
builder.Services.AddHttpClient<ITicketApiService, TicketApiService>();

// Register services
builder.Services.AddScoped<ITicketParserService, TicketParserService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
