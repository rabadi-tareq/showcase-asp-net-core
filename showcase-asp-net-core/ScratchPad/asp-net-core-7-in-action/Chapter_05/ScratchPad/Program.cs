var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var app = builder.Build();

app.UseDeveloperExceptionPage();

/* Intercepts standard 400 and 500 errors with no bodies and injects a user-friendly message in the response
unless the problem details service is injected, which then it will reformat the response
 */
app.UseStatusCodePages();
app.UseExceptionHandler("/error");

app.MapRazorPages();

app.MapGet("/throw-exception", () =>
{
    throw new Exception("Sample exception");
});

app.MapGet("/error", () => "Custom error handling logic");

app.MapGet("/return-not-found-with-body", () => Results.NotFound("Not found http error with body"));

// This endpoint returns a not found error with no body, which means it will be caught by the Use Status Code Pages
app.MapGet("/return-not-found", () => Results.NotFound());


app.Run();
