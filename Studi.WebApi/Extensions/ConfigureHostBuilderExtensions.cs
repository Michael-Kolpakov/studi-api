namespace Studi.WebApi.Extensions;

public static class ConfigureHostBuilderExtensions
{
    public static void ConfigureApplication(this ConfigureHostBuilder builder, WebApplicationBuilder appBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        appBuilder.Configuration.ConfigureCustom(environment);
    }
}
