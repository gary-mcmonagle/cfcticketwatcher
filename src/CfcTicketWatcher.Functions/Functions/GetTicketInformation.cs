using System.Text;
using CfcTicketWatcher.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CfcTicketWatcher.Functions.Functions;

/// <summary>
/// Azure Function that polls the Celtic FC ticket API and stores the response in blob storage.
/// Runs every 15 minutes.
/// </summary>
public class GetTicketInformation
{
    private readonly ITicketApiService _ticketApiService;
    private readonly ILogger<GetTicketInformation> _logger;

    public GetTicketInformation(
        ITicketApiService ticketApiService,
        ILogger<GetTicketInformation> logger)
    {
        _ticketApiService = ticketApiService;
        _logger = logger;
    }

    /// <summary>
    /// Timer-triggered function that fetches ticket data and outputs to blob storage
    /// </summary>
    [Function(nameof(GetTicketInformation))]
    [BlobOutput("ticket-data/responses/{DateTime}.json", Connection = "AzureWebJobsStorage")]
    public async Task<string?> Run(
        [TimerTrigger("0 */15 * * * *")] TimerInfo timerInfo,
        FunctionContext context)
    {
        _logger.LogInformation("GetTicketInformation function started at: {Time}", DateTime.UtcNow);

        if (timerInfo.IsPastDue)
        {
            _logger.LogWarning("Timer is past due, execution may be delayed");
        }

        try
        {
            var ticketData = await _ticketApiService.GetTicketDataAsync();

            if (string.IsNullOrEmpty(ticketData))
            {
                _logger.LogWarning("No ticket data received from API");
                return null;
            }

            _logger.LogInformation("Successfully retrieved ticket data ({Length} characters)", ticketData.Length);

            // Return the data to be written to blob storage
            return ticketData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch ticket information");
            throw;
        }
    }
}
