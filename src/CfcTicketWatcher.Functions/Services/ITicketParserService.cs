using CfcTicketWatcher.Models;

namespace CfcTicketWatcher.Functions.Services;

/// <summary>
/// Service for parsing ticket API responses and extracting match information
/// </summary>
public interface ITicketParserService
{
    /// <summary>
    /// Parses the raw JSON response and extracts upcoming matches
    /// </summary>
    /// <param name="json">Raw JSON from the ticket API</param>
    /// <returns>List of upcoming matches with ticket information</returns>
    List<UpcomingMatch> ParseUpcomingMatches(string json);

    /// <summary>
    /// Parses the raw JSON response into the typed API response
    /// </summary>
    /// <param name="json">Raw JSON from the ticket API</param>
    /// <returns>Parsed API response</returns>
    TicketApiResponse? ParseApiResponse(string json);
}
