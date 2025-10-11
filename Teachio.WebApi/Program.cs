using Teachio.WebApi.Extensions;

namespace Teachio.WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddApplicationServices(builder.Configuration, builder.Environment);

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseRouting();
        app.MapControllers();

        DatabaseExtension.InitializeDatabase(app);

        app.Run();
    }
}
