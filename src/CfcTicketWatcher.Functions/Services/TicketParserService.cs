using System.Text.Json;
using CfcTicketWatcher.Models;
using Microsoft.Extensions.Logging;

namespace CfcTicketWatcher.Functions.Services;

/// <summary>
/// Implementation of ITicketParserService for parsing Celtic FC ticket API responses
/// </summary>
public class TicketParserService : ITicketParserService
{
    private readonly ILogger<TicketParserService> _logger;

    public TicketParserService(ILogger<TicketParserService> logger)
    {
        _logger = logger;
    }

    public TicketApiResponse? ParseApiResponse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<TicketApiResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse ticket API response");
            return null;
        }
    }

    public List<UpcomingMatch> ParseUpcomingMatches(string json)
    {
        var matches = new List<UpcomingMatch>();

        var response = ParseApiResponse(json);
        if (response?.Body?.Content == null)
        {
            _logger.LogWarning("No content found in API response");
            return matches;
        }

        foreach (var row in response.Body.Content)
        {
            // Look for FixturesListWidget widgets which contain match data
            if (row.RowData == null) continue;

            foreach (var rowData in row.RowData)
            {
                if (rowData.WidgetType != "FixturesListWidget" || rowData.WidgetData?.Fixtures == null)
                    continue;

                foreach (var fixture in rowData.WidgetData.Fixtures)
                {
                    var match = ParseFixture(fixture, row.RowTitle ?? "Unknown Competition");
                    if (match != null)
                    {
                        matches.Add(match);
                    }
                }
            }
        }

        _logger.LogInformation("Parsed {Count} upcoming matches from API response", matches.Count);
        return matches;
    }

    private UpcomingMatch? ParseFixture(Fixture fixture, string competition)
    {
        if (string.IsNullOrEmpty(fixture.MatchID))
        {
            _logger.LogWarning("Fixture missing MatchID, skipping");
            return null;
        }

        var matchDetail = fixture.MatchDetails?.FirstOrDefault();
        if (matchDetail == null)
        {
            _logger.LogWarning("Fixture {MatchId} has no match details, skipping", fixture.MatchID);
            return null;
        }

        var match = new UpcomingMatch
        {
            PartitionKey = fixture.Season.ToString(),
            RowKey = fixture.MatchID,
            MatchLabel = matchDetail.MatchLabel ?? string.Empty,
            Competition = competition,
            SeasonName = matchDetail.SeasonName ?? string.Empty,
            TicketsAvailable = true, // If it's listed, tickets are available
            HomeCrest = matchDetail.MatchHomeCrest,
            AwayCrest = matchDetail.MatchAwayCrest,
            LastUpdated = DateTimeOffset.UtcNow,
            MatchDate = ParseMatchDate(matchDetail.MatchLabel)
        };

        return match;
    }

    /// <summary>
    /// Parses the match date from a label like "Celtic Vs. Falkirk - Sun, Feb 1st 2026, 15:00"
    /// </summary>
    private DateTimeOffset? ParseMatchDate(string? matchLabel)
    {
        if (string.IsNullOrEmpty(matchLabel))
            return null;

        try
        {
            // Extract the date part after " - "
            var datePart = matchLabel.Split(" - ", StringSplitOptions.RemoveEmptyEntries);
            if (datePart.Length < 2)
                return null;

            var dateString = datePart[1].Trim();

            // Remove ordinal suffixes (1st, 2nd, 3rd, 4th, etc.)
            dateString = System.Text.RegularExpressions.Regex.Replace(dateString, @"(\d+)(st|nd|rd|th)", "$1");

            // Parse "Sun, Feb 1 2026, 15:00"
            if (DateTimeOffset.TryParseExact(
                dateString,
                new[] { "ddd, MMM d yyyy, HH:mm", "ddd, MMM d yyyy HH:mm" },
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal,
                out var result))
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Could not parse date from match label: {MatchLabel}", matchLabel);
        }

        return null;
    }
}
