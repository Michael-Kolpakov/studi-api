using Serilog;
using Teachio.WebApi.Extensions;

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

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors();
        app.UseCustomSwagger();
        app.MapControllers();

        DatabaseExtension.InitializeDatabase(app);

        app.Run();
    }
}
