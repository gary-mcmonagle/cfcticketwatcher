using System.Text.Json;
using CFCTicketWatcher.Core.Domain.PageContent.Widgets;
using CFCTicketWatcher.Core.Result;

namespace CFCTicketWatcher.Core;

public interface IUpcomingFixtureService
{
    Task<List<FixtureResult>> GetUpcomingFixturesAsync();
}

public class UpcomingFixtureService(IPageContentService pageContentService, IFixtureService fixtureService) : IUpcomingFixtureService
{
    private const string TicketsPagePath = "tickets";

    public async Task<List<FixtureResult>> GetUpcomingFixturesAsync()
    {
        var results = new List<FixtureResult>();

        // Get the tickets page content
        var pageData = await pageContentService.GetPageContentAsync(TicketsPagePath);
        if (pageData?.Body?.Content == null)
        {
            return results;
        }

        // Extract all fixtures from FixturesListWidget widgets
        var fixtures = new List<Fixture>();
        foreach (var row in pageData.Body.Content)
        {
            if (row.RowData == null) continue;

            foreach (var gridItem in row.RowData)
            {
                if (gridItem.WidgetType == "FixturesListWidget" && gridItem.WidgetData.HasValue)
                {
                    var widgetData = gridItem.WidgetData.Value.Deserialize<FixturesListWidgetData>();
                    if (widgetData?.Fixtures != null)
                    {
                        fixtures.AddRange(widgetData.Fixtures);
                    }
                }
            }
        }

        // Get detailed fixture data for each fixture
        foreach (var fixture in fixtures)
        {
            if (string.IsNullOrEmpty(fixture.MatchID) || 
                !fixture.Season.HasValue || 
                string.IsNullOrEmpty(fixture.Squad))
            {
                continue;
            }

            var detailedFixture = await fixtureService.GetFixtureAsync(
                fixture.MatchID,
                fixture.Season.Value,
                fixture.Squad);

            if (detailedFixture?.Body == null) continue;

            var fixtureData = detailedFixture.Body;
            
            // Determine opponent based on home/away
            var opponent = GetOpponent(fixtureData);
            
            // Parse kickoff date
            var date = ParseKickOff(fixtureData.KickOffUTCTimestamp, fixtureData.KickOff);

            results.Add(new FixtureResult
            {
                Date = date,
                Opponent = opponent,
                Venue = fixtureData.Venue ?? string.Empty
            });
        }

        return results;
    }

    private static string GetOpponent(Domain.Fixture.FixtureData fixtureData)
    {
        if (fixtureData.TeamData == null || fixtureData.TeamData.Count < 2)
        {
            return "Unknown";
        }

        // Celtic's team ID is typically "t61" - find the other team
        var opponentTeam = fixtureData.TeamData.FirstOrDefault(t => t.TeamID != fixtureData.TeamID);
        return opponentTeam?.TeamName ?? "Unknown";
    }

    private static DateTime ParseKickOff(long? kickOffTimestamp, string? kickOff)
    {
        if (kickOffTimestamp.HasValue)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(kickOffTimestamp.Value).UtcDateTime;
        }

        if (!string.IsNullOrEmpty(kickOff) && DateTime.TryParse(kickOff, out var parsed))
        {
            return parsed;
        }

        return DateTime.MinValue;
    }
}
