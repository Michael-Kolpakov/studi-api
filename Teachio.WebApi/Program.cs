using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Serilog;
using Teachio.WebApi.Extensions;
using Teachio.WebApi.Middlewares;

namespace Teachio.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(context.Configuration).ReadFrom.Services(services));

        builder.Services.AddApplicationServices(builder.Configuration);

        var app = builder.Build();

        app.UseRequestLocalization(new RequestLocalizationOptions()
        {
            DefaultRequestCulture = new RequestCulture("en"),
            SupportedCultures = new[]
            {
                new CultureInfo("en")
            }
        });
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseCustomSwagger();
        app.MapControllers();

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        DatabaseExtension.InitializeDatabase(app);

        app.Run();
    }
}
