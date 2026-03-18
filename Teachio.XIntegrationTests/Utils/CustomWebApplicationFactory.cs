using Google.Apis.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Teachio.BLL.Models.Email.Base;
using Teachio.BLL.Services.Interfaces;
using Teachio.DAL.Entities.Users;
using Teachio.DAL.Persistence;
using Teachio.DAL.Utils.Constants;
using Teachio.DAL.Utils.Database;

namespace Teachio.XIntegrationTests.Utils;

/// <summary>
/// Represents the <see cref="CustomWebApplicationFactory{TProgram}"/> type.
/// </summary>
/// <typeparam name="TProgram">The type of program.</typeparam>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    public Mock<IEmailService> EmailServiceMock { get; private set; } = new Mock<IEmailService>();

    public Mock<IGoogleService> GoogleServiceMock { get; private set; } = new Mock<IGoogleService>();

    /// <summary>
    /// Performs the SetupMockEmailService operation.
    /// </summary>
    /// <param name="success">The <paramref name="success"/> argument.</param>
    public void SetupMockEmailService(bool success = true)
    {
        EmailServiceMock
            .Setup(es => es.SendEmailAsync(It.IsAny<MessageData>()))
            .ReturnsAsync(success);
    }

    /// <summary>
    /// Configures the Google token validation mock for the specified user.
    /// </summary>
    /// <param name="user">The <paramref name="user"/> argument.</param>
    /// <param name="token">The <paramref name="token"/> argument.</param>
    public void SetupMockGoogleLogin(AppUser user, string? token = null)
    {
        if (token is null)
        {
            GoogleServiceMock
                .Setup(gs => gs.ValidateGoogleTokenAsync("invalid_google_id_token"))
                .ThrowsAsync(new InvalidJwtException("Invalid Google Token"));
        }
        else
        {
            GoogleServiceMock
                .Setup(gs => gs.ValidateGoogleTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(new GoogleJsonWebSignature.Payload()
                {
                    Email = user.Email,
                    GivenName = user.Name,
                    FamilyName = user.Surname,
                    Subject = "google-subject-id"
                });
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");

        builder.ConfigureServices(services =>
        {
            ConfigureEmailService(services, EmailServiceMock);
            ConfigureGoogleService(services, GoogleServiceMock);
            ConfigureDatabase(services);
        });
    }

    private static void ConfigureEmailService(IServiceCollection services, Mock<IEmailService> emailServiceMock)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IEmailService));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton(emailServiceMock.Object);
    }

    private static void ConfigureGoogleService(IServiceCollection services, Mock<IGoogleService> googleServiceMock)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IGoogleService));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddSingleton(googleServiceMock.Object);
    }

    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddSingleton<InMemoryDatabaseRoot>(_ => IntegrationTestDatabaseRoot.Root);

        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TeachioDbContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<TeachioDbContext>((serviceProvider, options) =>
        {
            var root = serviceProvider.GetRequiredService<InMemoryDatabaseRoot>();
            options.UseInMemoryDatabase(DatabaseConstants.IntegrationTestsInMemoryDatabase, root);
        });

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TeachioDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
