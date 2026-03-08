using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CFCTicketWatcher.Func.Functions;

public class FixtureHttp(ILogger<FixtureHttp> logger)
{
    [Function(nameof(FixtureHttp))]
    public FixtureHttpOutput Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        var requestId = Guid.NewGuid().ToString();
        
        logger.LogInformation("FixtureHttp triggered manually at: {Time}. Sending request {RequestId}", 
            DateTime.UtcNow, requestId);

        return new FixtureHttpOutput
        {
            HttpResponse = new OkObjectResult(new { requestId, message = "Fixture request queued successfully" }),
            QueueMessage = requestId
        };
    }
}

public class FixtureHttpOutput
{
    [HttpResult]
    public IActionResult HttpResponse { get; set; } = null!;

    [QueueOutput("fixture-requests")]
    public string QueueMessage { get; set; } = null!;
}
