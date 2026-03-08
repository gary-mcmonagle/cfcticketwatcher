using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CFCTicketWatcher.Func.Functions;

public class FixtureTimer(ILogger<FixtureTimer> logger)
{
    [Function(nameof(FixtureTimer))]
    [QueueOutput("fixture-requests")]
    public string Run([TimerTrigger("0 0 */8 * * *")] TimerInfo timerInfo)
    {
        var requestId = Guid.NewGuid().ToString();
        
        logger.LogInformation("FixtureTimer triggered at: {Time}. Sending request {RequestId}", 
            DateTime.UtcNow, requestId);

        return requestId;
    }
}
