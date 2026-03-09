using Azure.Communication.Email;
using Azure.Data.Tables;
using CFCTicketWatcher.Core;
using CFCTicketWatcher.Func.TableStorage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Register Azure Communication Services Email client
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration["AzureCommunicationServicesConnectionString"];
    if (string.IsNullOrEmpty(connectionString))
    {
        // Return a client that will fail at runtime if connection string is not configured
        return new EmailClient("endpoint=https://placeholder.communication.azure.com/;accesskey=placeholder");
    }
    return new EmailClient(connectionString);
});

// Register Azure Table Storage client for fixture tracking
builder.Services.AddSingleton(sp =>
{
    var connectionString = builder.Configuration["AzureWebJobsStorage"];
    if (string.IsNullOrEmpty(connectionString))
    {
        return new TableServiceClient("UseDevelopmentStorage=true");
    }
    return new TableServiceClient(connectionString);
});

builder.Services.AddSingleton<IFixtureTableStorageService, FixtureTableStorageService>();

// Register HTTP clients for Core services with retry policy
builder.Services.AddHttpClient<IPageContentService, PageContentService>(client =>
{
    client.BaseAddress = new Uri("https://webapi.gc.celticfc.com/");
}).AddRetryPolicy();

builder.Services.AddHttpClient<IFixtureService, FixtureService>(client =>
{
    client.BaseAddress = new Uri("https://webapi.gc.celticfc.com/");
}).AddRetryPolicy();

builder.Services.AddTransient<IUpcomingFixtureService, UpcomingFixtureService>();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
