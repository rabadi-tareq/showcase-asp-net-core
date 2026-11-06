using Microsoft.AspNetCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Registers Problem Details services (IProblemDetailsService) in the DI container.
// Must be called before builder.Build() while the service collection is still open for modifications.
builder.Services.AddProblemDetails(options =>
{
    // CustomizeProblemDetails runs whenever a ProblemDetails payload is generated.
    options.CustomizeProblemDetails = context =>
    {
        // Correlate the client error response with server-side logs.
        // Serialized as a top-level extension property in the RFC 7807 JSON response (e.g., "traceId": "...").
        context.ProblemDetails.Extensions["traceId"] =
            context.HttpContext.TraceIdentifier;

        // Retrieve the unhandled exception captured by the exception handling middleware.
        var exceptionFeature =
            context.HttpContext.Features.Get<IExceptionHandlerFeature>();

        var exception = exceptionFeature?.Error;

        // Map TimeoutException to 503 Service Unavailable instead of default 500 Internal Server Error.
        if (exception is TimeoutException)
        {
            // Update the RFC 7807 payload status and title
            context.ProblemDetails.Status = StatusCodes.Status503ServiceUnavailable;
            context.ProblemDetails.Title = "Service temporarily unavailable";

            // Update the actual HTTP wire response status code
            context.HttpContext.Response.StatusCode =
                StatusCodes.Status503ServiceUnavailable;
        }
    };
});

var app = builder.Build();

// Intercepts unhandled exceptions caught further down in the pipeline and routes them to a dedicated error sub-pipeline.
app.UseExceptionHandler(errorApp =>
{
    // Intermediate middleware: executes for all handled exceptions before terminal formatting.
    // Attaches a custom diagnostic header so clients, proxies, and logs know this error was processed by the app pipeline.
    errorApp.Use(async (context, next) =>
    {
        context.Response.Headers["X-Error-Handled-By"] = "ExceptionPipeline";
        await next();
    });

    // Terminal middleware: terminates the error sub-pipeline and produces the response body.
    errorApp.Run(async context =>
    {
        // Content negotiation: check if the client explicitly requested plain text.
        if (context.Request.Headers.Accept.Contains("text/plain"))
        {
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("Something went wrong");
            return;
        }

        // Default: return an RFC 7807 Problem Details response (which triggers CustomizeProblemDetails configured above).
        await Results.Problem("Something went wrong")
            .ExecuteAsync(context);
    });
});


app.Run();