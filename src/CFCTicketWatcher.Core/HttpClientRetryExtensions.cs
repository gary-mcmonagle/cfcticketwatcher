using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Polly;
using Polly.Extensions.Http;

namespace CFCTicketWatcher.Core;

public static class HttpClientRetryExtensions
{
    /// <summary>
    /// Adds a retry policy with exponential backoff to the HttpClient.
    /// Retries 3 times with delays of 1s, 2s, and 4s.
    /// </summary>
    public static IHttpClientBuilder AddRetryPolicy(this IHttpClientBuilder builder)
    {
        return builder.AddPolicyHandler(GetRetryPolicy());
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles HttpRequestException, 5xx, and 408 responses
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt - 1)), // 1s, 2s, 4s
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Logging handled by the caller if needed
                });
    }
}
