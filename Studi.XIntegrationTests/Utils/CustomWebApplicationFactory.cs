using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Studi.BLL.Models.Email.Base;
using Studi.BLL.Services.Interfaces;
using Studi.DAL.Persistence;
using Studi.DAL.Utils.Constants;
using Studi.DAL.Utils.Database;

namespace Studi.XIntegrationTests.Utils;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    public Mock<IEmailService> EmailServiceMock { get; private set; } = new Mock<IEmailService>();

    public void SetupMockEmailService(bool success = true)
    {
        EmailServiceMock
            .Setup(es => es.SendEmailAsync(It.IsAny<MessageData>()))
            .ReturnsAsync(success);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");

        builder.ConfigureServices(services =>
        {
            ConfigureEmailService(services, EmailServiceMock);
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

    private static void ConfigureDatabase(IServiceCollection services)
    {
        services.AddSingleton<InMemoryDatabaseRoot>(_ => IntegrationTestDatabaseRoot.Root);

        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<StudiDbContext>));

        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }

        services.AddDbContext<StudiDbContext>((serviceProvider, options) =>
        {
            var root = serviceProvider.GetRequiredService<InMemoryDatabaseRoot>();
            options.UseInMemoryDatabase(DatabaseConstants.IntegrationTestsInMemoryDatabase, root);
        });

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<StudiDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
