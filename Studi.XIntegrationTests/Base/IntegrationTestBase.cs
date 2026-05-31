using Microsoft.Extensions.Configuration;
using Studi.WebApi.Extensions;

namespace Studi.XIntegrationTests.Base;

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
