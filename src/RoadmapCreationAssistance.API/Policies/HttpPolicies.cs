using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace RoadmapCreationAssistance.API.Policies;

public class HttpPolicies(ILogger<HttpPolicies> logger)
{
    public IAsyncPolicy<HttpResponseMessage> GetGitHubRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, response, context) =>
                {
                    // Respect GitHub's Retry-After header if present
                    if (response.Result?.Headers.RetryAfter?.Delta is TimeSpan retryAfter)
                        return retryAfter;

                    // Exponential backoff: 2s, 4s, 8s
                    return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                },
                onRetryAsync: (outcome, timespan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        "GitHub API retry {RetryAttempt} after {Delay}s. Reason: {Reason}",
                        retryAttempt,
                        timespan.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    return Task.CompletedTask;
                });
    }

    public IAsyncPolicy<HttpResponseMessage> GetOpenAIRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, response, context) =>
                {
                    // Respect Retry-After header if present
                    if (response.Result?.Headers.RetryAfter?.Delta is TimeSpan retryAfter)
                        return retryAfter;

                    // Longer backoff for OpenAI: 3s, 9s, 27s
                    return TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                },
                onRetryAsync: (outcome, timespan, retryAttempt, context) =>
                {
                    logger.LogWarning(
                        "OpenAI API retry {RetryAttempt} after {Delay}s. Reason: {Reason}",
                        retryAttempt,
                        timespan.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result?.StatusCode.ToString());
                    return Task.CompletedTask;
                });
    }
}