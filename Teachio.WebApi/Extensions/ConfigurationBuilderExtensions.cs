namespace Teachio.WebApi.Extensions;

/// <summary>
/// Represents the <see cref="ConfigurationBuilderExtensions"/> type.
/// </summary>
public static class ConfigurationBuilderExtensions
{
    /// <summary>
    /// Configures the target component.
    /// </summary>
    /// <param name="builder">The <paramref name="builder"/> argument.</param>
    /// <param name="environment">The <paramref name="environment"/> argument.</param>
    /// <returns>The result produced by this operation.</returns>
    public static IConfigurationBuilder ConfigureCustom(this IConfigurationBuilder builder, string environment)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

        return builder;
    }
}
