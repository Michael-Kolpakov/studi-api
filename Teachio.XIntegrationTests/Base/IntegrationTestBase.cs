using Microsoft.Extensions.Configuration;
using Teachio.WebApi.Extensions;

namespace Teachio.XIntegrationTests.Base;

/// <summary>
/// Represents the <see cref="IntegrationTestBase"/> type.
/// </summary>
public class IntegrationTestBase
{
    public IntegrationTestBase()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTests");
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        var configurationBuilder = new ConfigurationBuilder()
            .ConfigureCustom(environment);

        Configuration = configurationBuilder.Build();
    }

    protected IConfiguration Configuration { get; }
}
