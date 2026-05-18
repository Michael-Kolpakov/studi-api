using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Teachio.BLL.Models.Auth;
using Teachio.BLL.Models.Media;
using Teachio.BLL.Models.Storage;
using Teachio.BLL.Services.Interfaces;
using Teachio.BLL.Services.Realizations;
using Teachio.BLL.SharedResource;
using Teachio.DAL.Repositories.Interfaces.Base;
using Teachio.DAL.Repositories.Realizations.Base;
using Teachio.DAL.SharedResource;
using Teachio.WebApi.Services;

namespace Teachio.WebApi.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var bllAssemblyName = typeof(BllSharedResource).Assembly.GetName().Name!;
        var bllAssembly = Assembly.Load(bllAssemblyName);
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
        services.AddAuthenticationServices(configuration);
        services.AddSwagger();
        services.AddSerilogLogging();
        services.AddOptions(configuration);
        services.AddCustomServices();
        services.AddRepositoryServices();
        services.AddAutoMapper(_ => { }, currentAssemblies);
        services.AddMediatR(config => config.RegisterServicesFromAssemblies(bllAssembly));
        services.AddCors();
        services.AddHttpContextAccessor();

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
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEntityExistenceService, EntityExistenceService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuthTokenIssuer, AuthTokenIssuer>();
        services.AddScoped<IGoogleDriveStorageService, GoogleDriveStorageService>();
        services.AddScoped<IVideoMetadataService, VideoMetadataService>();
        services.AddScoped<IThumbnailMetadataService, ThumbnailMetadataService>();
    }

    private static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleDriveStorageOptions>(
            configuration.GetSection(GoogleDriveStorageOptions.SectionName));

        services.Configure<FfprobeOptions>(
            configuration.GetSection(FfprobeOptions.SectionName));

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));
    }

    private static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }
}
