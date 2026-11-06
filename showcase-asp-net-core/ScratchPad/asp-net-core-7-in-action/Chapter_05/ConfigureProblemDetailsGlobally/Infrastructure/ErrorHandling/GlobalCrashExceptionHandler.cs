using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConfigureProblemDetailsGlobally.Infrastructure.ErrorHandling
{
    // Handles actual crashes (e.g., NullReferenceException, database timeouts)
    public sealed class GlobalCrashExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalCrashExceptionHandler> logger,
        IHostEnvironment env) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "An unhandled exception occurred.");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Detail = env.IsDevelopment() ? exception.ToString() : "An unexpected error occurred."
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
        }
    }
}
