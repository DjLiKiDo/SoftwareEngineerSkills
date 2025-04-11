using Microsoft.OpenApi.Models;

namespace SoftwareEngineerSkills.API.Extensions;

/// <summary>
/// Extension methods for setting up OpenAPI in the application
/// </summary>
public static class OpenApiExtensions
{
    /// <summary>
    /// Adds OpenAPI/Swagger services to the application
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddCustomOpenApi(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Software Engineer Skills API",
                Version = "v1",
                Description = "API for Software Engineer Skills"
            });
        });

        return services;
    }

    /// <summary>
    /// Maps OpenAPI/Swagger endpoints in the application
    /// </summary>
    /// <param name="app">The web application</param>
    public static void MapCustomOpenApi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "SoftwareEngineerSkills API v1");
        });
    }
}
