using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http.HttpResults;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddProblemDetails();

WebApplication app = builder.Build();


var _fruit = new ConcurrentDictionary<string, Fruit>();


app.MapGet("/", () => "Welcome to the Fruit API!");


app.MapGet("/fruit", () => _fruit);

app.MapGet("/fruit/{id}", (string id) =>
    _fruit.TryGetValue(id, out var fruit)
        ? TypedResults.Ok(fruit)
        : Results.Problem(statusCode: 404));

app.MapGet("/fruit/2/{name}", (string name) => $"You have requested a resource for {name}");

/*
 * The resource location is returned in the created response so it is easier to know where to find the resource in the following scenarios
 * the frontend needs to navigate to the resource without having to parse the json body, like a multi-step booking process
 * Standardized API gateway and reverse proxies
 */
app.MapPost("/fruit/{id}", (string id, Fruit fruit) =>
    _fruit.TryAdd(id, fruit)
        ? TypedResults.Created($"/fruit/{id}", fruit)
        : Results.BadRequest(new
        { id = "A fruit with this id already exists" }));

app.MapPut("/fruit/{id}", (string id, Fruit fruit) =>
{
    _fruit[id] = fruit;
    return Results.NoContent();
});

app.MapDelete("/fruit/{id}", (string id) =>
{
    _fruit.TryRemove(id, out _);
    return Results.NoContent();
});


app.MapGet("/users/{id}", Results<Ok<UserDto>, NotFound> (int id, IUserService service) =>
{
    var user = service.Find(id);
    return user is not null
        ? TypedResults.Ok(user)
        : TypedResults.NotFound();

    // The compiler ensures you only return Ok or NotFound
});

// app.UseExceptionHandlerMiddleware
app.Run();
record Fruit(string Name, int stock);

public record UserDto(string id);

public interface IUserService
{
    public UserDto Find(int id);
}

public class UserService : IUserService
{
    public UserDto Find(int id)
    {
        // Dummy implementation
        return new UserDto(id.ToString());
    }
}