using ConfigureProblemDetailsGlobally.Infrastructure.ErrorHandling;

var builder = WebApplication.CreateBuilder(args);

// Register the exception handlers in their explicit order
builder.Services.AddApiExceptionHandlers();

// Register the central Problem Details service and our global formatter
builder.Services.AddProblemDetails();
builder.Services.ConfigureOptions<ProblemDetailsSetup>();

builder.Services.AddRazorPages();

var app = builder.Build();

// Intercepts
// But if the Problem Details Service is injected, the service takes over and returns a problem details response.
app.UseStatusCodePages();

// Catches all application-wide unhandled exceptions and provides a unified centerlized location for handling them
// But if the problem details service is injected, the servoce tales pver and returns a problem details response
app.UseExceptionHandler();

app.MapRazorPages();

app.MapGet("/throw-exception", () => {
    throw new Exception("Something went wrong behind the scenes!");
});

app.MapGet("/return-not-found-with-body", () => Results.NotFound("custom not found"));
app.MapGet("/return-not-found-with-no-body", () => Results.NotFound());

app.Run();
