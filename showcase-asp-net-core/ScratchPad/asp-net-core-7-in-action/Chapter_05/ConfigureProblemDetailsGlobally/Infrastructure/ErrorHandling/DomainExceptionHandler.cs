using ConfigureProblemDetailsGlobally.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConfigureProblemDetailsGlobally.Infrastructure.ErrorHandling
{
    // Handles all business logic violations
    public sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not DomainException domainException)
                return false; // Let the next handler try

            var (statusCode, type, title) = domainException switch
            {
                InsufficientFundsException => (StatusCodes.Status400BadRequest, "https://api.yourdomain.com/errors/funds", "Insufficient Funds"),
                EntityNotFoundException => (StatusCodes.Status404NotFound, "https://api.yourdomain.com/errors/not-found", "Resource Not Found"),
                _ => (StatusCodes.Status400BadRequest, "https://api.yourdomain.com/errors/domain-rule", "Domain Rule Violation")
            };

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Type = type,
                Detail = domainException.Message
            };

            httpContext.Response.StatusCode = statusCode;

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = problemDetails,
                Exception = exception
            });
        }
    }
}
