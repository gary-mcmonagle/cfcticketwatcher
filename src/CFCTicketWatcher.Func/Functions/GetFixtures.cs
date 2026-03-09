using CFCTicketWatcher.Core;
using CFCTicketWatcher.Func.TableStorage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CFCTicketWatcher.Func.Functions;

public class GetFixtures(
    ILogger<GetFixtures> logger, 
    IUpcomingFixtureService upcomingFixtureService,
    IFixtureTableStorageService fixtureTableStorageService)
{
    [Function(nameof(GetFixtures))]
    public async Task<GetFixturesOutput> Run([QueueTrigger("fixture-requests")] string requestId)
    {
        logger.LogInformation("Processing fixture request: {RequestId}", requestId);

        try
        {
            var fixtures = await upcomingFixtureService.GetUpcomingFixturesAsync();
            
            logger.LogInformation("Found {Count} upcoming fixtures for request {RequestId}", 
                fixtures.Count, requestId);

            var storedFixtures = await fixtureTableStorageService.GetStoredFixturesAsync();
            var storedRowKeys = storedFixtures
                .Select(FixtureTableStorageService.BuildRowKey)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newFixtures = fixtures
                .Where(f => !storedRowKeys.Contains(FixtureTableStorageService.BuildRowKey(f)))
                .ToList();

            logger.LogInformation("Detected {NewCount} new fixture(s) for request {RequestId}",
                newFixtures.Count, requestId);

            await fixtureTableStorageService.UpdateFixturesAsync(fixtures);

            return new GetFixturesOutput
            {
                Result = new FixtureResultMessage
                {
                    RequestId = requestId,
                    Fixtures = fixtures,
                    NewFixtures = newFixtures,
                    ProcessedAt = DateTime.UtcNow
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing fixture request {RequestId}", requestId);
            
            return new GetFixturesOutput
            {
                Failure = new FailureMessage
                {
                    RequestId = requestId,
                    FunctionName = nameof(GetFixtures),
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    FailedAt = DateTime.UtcNow
                }
            };
        }
    }
}

public class GetFixturesOutput
{
    [QueueOutput("fixture-results")]
    public FixtureResultMessage? Result { get; set; }
    
    [QueueOutput("fixture-failures")]
    public FailureMessage? Failure { get; set; }
}

public class FixtureResultMessage
{
    public string RequestId { get; init; } = string.Empty;
    public List<Core.Result.FixtureResult> Fixtures { get; init; } = [];
    public List<Core.Result.FixtureResult> NewFixtures { get; init; } = [];
    public DateTime ProcessedAt { get; init; }
}
