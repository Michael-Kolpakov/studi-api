using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.Services.Realizations;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Realizations.Base;
using Teachio.DAL.SharedResource;

namespace Teachio.WebApi.Extensions;

public static class ApplicationServicesExtensions
{
    private const string BllAssemblyName = "Teachio.BLL";

    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var bllAssembly = Assembly.Load(BllAssemblyName);
        var currentAssemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Concat([bllAssembly])
            .Distinct()
            .ToArray();

        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));
        }).AddDataAnnotationsLocalization(options =>
        {
            options.DataAnnotationLocalizerProvider = (_, factory) =>
                factory.Create(typeof(DataAnnotationsSharedResource));
        });
        services.AddCustomDbContext(configuration);
        services.AddSwagger();
        services.AddSerilogLogging();
        services.AddCustomServices();
        services.AddRepositoryServices();
        services.AddAutoMapper(_ => { }, currentAssemblies);
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(bllAssembly));
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

    private static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<ILoggerService, LoggerService>();
        services.AddScoped<IEntityExistenceService, EntityExistenceService>();
    }

    private static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }
}
