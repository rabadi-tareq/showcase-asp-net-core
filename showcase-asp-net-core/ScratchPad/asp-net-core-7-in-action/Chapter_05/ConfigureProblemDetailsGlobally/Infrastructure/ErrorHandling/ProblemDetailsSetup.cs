using Microsoft.Extensions.Options;

namespace ConfigureProblemDetailsGlobally.Infrastructure.ErrorHandling
{
    public class ProblemDetailsSetup : IConfigureOptions<ProblemDetailsOptions>
    {
        public void Configure(ProblemDetailsOptions options)
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
                context.ProblemDetails.Extensions.TryAdd("occured_on", DateTime.Now.Date.ToShortDateString());
            };
        }
    }
}
