using Microsoft.AspNetCore.Mvc;

namespace Teachio.WebApi.Extensions;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));
        });
        services.AddCustomDbContext(configuration);
        services.AddSwagger();
        services.AddSerilogLogging();
        services.AddAutoMapper(currentAssemblies);
        services.AddCors();

        return services;
    }

    private static void AddCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            { 
                policy
                    .WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }
}
