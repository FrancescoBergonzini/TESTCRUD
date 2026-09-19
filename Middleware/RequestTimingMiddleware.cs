using System.Diagnostics;

public sealed class RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger, IConfiguration configuration)
{
    private readonly int slowRequestThresholdMs = configuration.GetValue("Performance:SlowRequestThresholdMs", 500);

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsedMs = stopwatch.Elapsed.TotalMilliseconds;
            if (!context.Response.HasStarted)
            {
                context.Response.Headers["X-Response-Time-ms"] = elapsedMs.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            }

            var logLevel = elapsedMs >= slowRequestThresholdMs ? LogLevel.Warning : LogLevel.Information;
            logger.Log(logLevel, "HTTP {Method} {Path} returned {StatusCode} in {ElapsedMs:F2} ms",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, elapsedMs);
        }
    }
}
