namespace CfcTicketWatcher.Functions.Services;

/// <summary>
/// Service for fetching ticket data from the Celtic FC API
/// </summary>
public interface ITicketApiService
{
    /// <summary>
    /// Fetches the raw ticket information JSON from the API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Raw JSON response as string</returns>
    Task<string> GetTicketDataAsync(CancellationToken cancellationToken = default);
}
