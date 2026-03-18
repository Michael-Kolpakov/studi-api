namespace Teachio.WebApi.Extensions;

/// <summary>
/// Represents the <see cref="ConfigureHostBuilderExtensions"/> type.
/// </summary>
public static class ConfigureHostBuilderExtensions
{
    /// <summary>
    /// Configures the target component.
    /// </summary>
    /// <param name="builder">The <paramref name="builder"/> argument.</param>
    /// <param name="appBuilder">The <paramref name="appBuilder"/> argument.</param>
    public static void ConfigureApplication(this ConfigureHostBuilder builder, WebApplicationBuilder appBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        appBuilder.Configuration.ConfigureCustom(environment);
    }
}
