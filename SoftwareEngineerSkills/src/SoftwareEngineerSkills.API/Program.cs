using SoftwareEngineerSkills.API;
using SoftwareEngineerSkills.API.Extensions;
using SoftwareEngineerSkills.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApiServices();

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

// Add global exception handling middleware (should be early in the pipeline)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
