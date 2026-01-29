using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CfcTicketWatcher.Functions.Services;

/// <summary>
/// Implementation of ITicketApiService that fetches data from the Celtic FC API
/// </summary>
public class TicketApiService : ITicketApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TicketApiService> _logger;
    private readonly string _ticketApiUrl;

    public TicketApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TicketApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _ticketApiUrl = configuration["TicketApiUrl"] 
            ?? "https://webapi.gc.celticfc.com/v1/pages/byfullpath?fullPath=tickets";
    }

    public async Task<string> GetTicketDataAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching ticket data from {Url}", _ticketApiUrl);

        try
        {
            var response = await _httpClient.GetAsync(_ticketApiUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Successfully fetched {Length} characters of ticket data", content.Length);

            return content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch ticket data from {Url}", _ticketApiUrl);
            throw;
        }
    }
}
