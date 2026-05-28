using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Studi.BLL.Models.Auth;
using Studi.BLL.Models.Email;
using Studi.BLL.Models.Media;
using Studi.BLL.Models.Storage;
using Studi.BLL.Services.Interfaces;
using Studi.BLL.Services.Realizations;
using Studi.BLL.SharedResource;
using Studi.DAL.Repositories.Interfaces.Base;
using Studi.DAL.Repositories.Realizations.Base;
using Studi.DAL.SharedResource;

namespace Studi.WebApi.Extensions;

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
        services.AddScoped<ICourseAccessService, CourseAccessService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IAuthTokenIssuer, AuthTokenIssuer>();
        services.AddScoped<IPinCodeService, PinCodeService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IGoogleDriveStorageService, GoogleDriveStorageService>();
        services.AddScoped<IVideoMetadataService, VideoMetadataService>();
        services.AddScoped<IThumbnailMetadataService, ThumbnailMetadataService>();
        services.AddScoped<IAvatarMetadataService, AvatarMetadataService>();
    }

    private static void AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GoogleDriveStorageOptions>(
            configuration.GetSection(GoogleDriveStorageOptions.SectionName));

        services.Configure<FfprobeOptions>(
            configuration.GetSection(FfprobeOptions.SectionName));

        services.Configure<JwtOptions>(
            configuration.GetSection(JwtOptions.SectionName));

        services.Configure<SmtpOptions>(
            configuration.GetSection(SmtpOptions.SectionName));

        services.Configure<EmailVerificationOptions>(
            configuration.GetSection(EmailVerificationOptions.SectionName));
    }

    private static void AddRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
    }
}
