using System.Reflection;
using Microsoft.OpenApi.Models;

namespace Teachio.WebApi.Extensions;

/// <summary>
/// Represents the <see cref="SwaggerExtensions"/> type.
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Creates a new instance in the target store.
    /// </summary>
    /// <param name="services">The <paramref name="services"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TeachioApi",
                Version = "v1"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            options.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    /// <summary>
    /// Performs the <see cref="UseCustomSwagger"/> operation.
    /// </summary>
    /// <param name="app">The <paramref name="app"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "TeachAPI V1"));

        return app;
    }
}
