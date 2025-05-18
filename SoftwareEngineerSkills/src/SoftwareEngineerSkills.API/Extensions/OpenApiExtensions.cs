using Microsoft.OpenApi.Models;

namespace SoftwareEngineerSkills.API.Extensions;

public static class OpenApiExtensions
{
    public static WebApplication MapCustomOpenApi(this WebApplication app)
    {
        // Placeholder for Swagger/OpenAPI configuration
        // In a real scenario, this would configure Swagger UI and JSON endpoint.
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            // Configure Swagger UI options here, e.g., versions
            // options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        });
        return app;
    }

    public static IServiceCollection AddOpenApiServices(this IServiceCollection services)
    {
        // Placeholder for adding Swagger/OpenAPI services
        services.AddEndpointsApiExplorer(); // Needed for minimal APIs, good to have
        services.AddSwaggerGen(options =>
        {
            // Configure SwaggerGen options here, e.g., document information, security definitions
            // options.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });
        return services;
    }
}
