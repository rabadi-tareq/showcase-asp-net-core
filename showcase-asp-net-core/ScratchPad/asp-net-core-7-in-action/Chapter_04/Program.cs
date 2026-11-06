WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.UseExceptionHandler("/error");

// additional middleware configuration
app.MapGet("/error", () => "Sorry, an error occurred");

app.MapGet("/GenerateErrror", () => { throw new Exception(); });

app.Run();