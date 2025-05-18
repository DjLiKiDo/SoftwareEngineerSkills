using SoftwareEngineerSkills.API;
using SoftwareEngineerSkills.API.Extensions;
using SoftwareEngineerSkills.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiServices(builder.Configuration); // Pass configuration

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Configure Swagger for development
    app.MapCustomOpenApi();
}
else
{
    // In production, enforce HTTPS
    app.UseHsts();
}

// Use exception handler middleware (modern approach)
app.UseExceptionHandler();

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// Make Program class accessible to tests
public partial class Program {}
