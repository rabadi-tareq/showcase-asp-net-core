/*
 *Create a WebApplicationBuilder instance
 * Register the required service and configuration with the WebApplicationBuilder instance
 * Call Build on the builder instance to create and instance of WebApplication
 * Create the Middleware pipeline
 * Map the endpoints (minimal or traditional)
 * Call Run on the WebApplication instance to start listening to requests
 */

// Import the namespace for HTTP logging functionality
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging.EventLog;

// Create a WebApplicationBuilder instance with command-line arguments
// This builder is used to configure services and the application pipeline
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register HTTP logging services in the dependency injection container
// Configure the logging options to capture all HTTP request and response fields
builder.Services.AddHttpLogging(opts =>
    opts.LoggingFields = HttpLoggingFields.All);

// Add a logging filter to ensure HTTP logging messages are written at Information level
// This ensures that HTTP logs from Microsoft.AspNetCore.HttpLogging are captured
builder.Logging.AddFilter<DebugLoggerProvider>(
    "Microsoft.AspNetCore.HttpLogging", LogLevel.Information);

// Build the WebApplication instance from the configured builder
// This creates the application with all registered services and configurationsmd
WebApplication app = builder.Build();

// Check if the application is running in the Development environment
// HTTP logging is only enabled in development to avoid performance overhead in production
if (app.Environment.IsDevelopment())
{
    // Add HTTP logging middleware to the request pipeline
    // This middleware logs details about incoming requests and outgoing responses
    app.UseHttpLogging();
}

// Map a GET endpoint at the root path "/" that returns a simple text response
app.MapGet("/", () => "Hello World!");

// Map a GET endpoint at "/person" that returns a Person object as JSON
app.MapGet("/person", () => new Person("Andrew", "Lock"));

// Start the web application and begin listening for HTTP requests
// This method blocks until the application is shut down
app.Run();

// Define a Person record type with FirstName and LastName properties
// Records provide immutable data structures with value-based equality
public record Person(string FirstName, string LastName);