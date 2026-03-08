using CFCTicketWatcher.Core;
using Microsoft.Extensions.DependencyInjection;

// Set up DI container
var services = new ServiceCollection();

// Register HttpClient with base URL for the Celtic FC API
services.AddHttpClient<IPageContentService, PageContentService>(client =>
{
    client.BaseAddress = new Uri("https://webapi.gc.celticfc.com/");
});

services.AddHttpClient<IFixtureService, FixtureService>(client =>
{
    client.BaseAddress = new Uri("https://webapi.gc.celticfc.com/");
});

services.AddTransient<IUpcomingFixtureService, UpcomingFixtureService>();

var serviceProvider = services.BuildServiceProvider();

// Get the service and fetch upcoming fixtures
var upcomingFixtureService = serviceProvider.GetRequiredService<IUpcomingFixtureService>();

Console.WriteLine("Fetching upcoming fixtures from Celtic FC API...\n");

var fixtures = await upcomingFixtureService.GetUpcomingFixturesAsync();

if (fixtures.Count > 0)
{
    Console.WriteLine($"Found {fixtures.Count} upcoming fixtures:\n");
    Console.WriteLine($"{"Date",-25} {"Opponent",-25} {"Venue"}");
    Console.WriteLine(new string('-', 75));
    
    foreach (var fixture in fixtures)
    {
        Console.WriteLine($"{fixture.Date.ToString("ddd, MMM d yyyy HH:mm"),-25} {fixture.Opponent,-25} {fixture.Venue}");
    }
}
else
{
    Console.WriteLine("No upcoming fixtures found.");
}
