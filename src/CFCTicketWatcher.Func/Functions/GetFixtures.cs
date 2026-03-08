using CFCTicketWatcher.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CFCTicketWatcher.Func.Functions;

public class GetFixtures(
    ILogger<GetFixtures> logger, 
    IUpcomingFixtureService upcomingFixtureService)
{
    [Function(nameof(GetFixtures))]
    [QueueOutput("fixture-results")]
    public async Task<FixtureResultMessage?> Run([QueueTrigger("fixture-requests")] string requestId)
    {
        logger.LogInformation("Processing fixture request: {RequestId}", requestId);

        try
        {
            var fixtures = await upcomingFixtureService.GetUpcomingFixturesAsync();
            
            logger.LogInformation("Found {Count} upcoming fixtures for request {RequestId}", 
                fixtures.Count, requestId);

            return new FixtureResultMessage
            {
                RequestId = requestId,
                Fixtures = fixtures,
                ProcessedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing fixture request {RequestId}", requestId);
            throw;
        }
    }
}

public class FixtureResultMessage
{
    public string RequestId { get; init; } = string.Empty;
    public List<Core.Result.FixtureResult> Fixtures { get; init; } = [];
    public DateTime ProcessedAt { get; init; }
}
