using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using FluentResults;
using Studi.BLL.Services.Interfaces;

namespace Studi.WebApi.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<Type, int> _exceptionStatusMap;

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));

        _exceptionStatusMap = new Dictionary<Type, int>()
        {
            { typeof(ValidationException), StatusCodes.Status400BadRequest },
            { typeof(ArgumentException), StatusCodes.Status400BadRequest },
            { typeof(ArgumentNullException), StatusCodes.Status400BadRequest },
            { typeof(UnauthorizedAccessException), StatusCodes.Status401Unauthorized },
            { typeof(KeyNotFoundException), StatusCodes.Status404NotFound },
            { typeof(InvalidOperationException), StatusCodes.Status409Conflict },
        };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILoggerService>();
        logger.LogError(null, ex.Message, ex.StackTrace);

        var statusCode = _exceptionStatusMap.TryGetValue(ex.GetType(), out var mapped)
            ? mapped
            : StatusCodes.Status500InternalServerError;

        var result = Result.Fail(ex.Message);

        var payload = new
        {
            succeeded = result.IsSuccess,
            errors = result.Errors.Select(e => new
            {
                message = e.Message,
                metadata = e.Metadata.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            })
        };

        context.Response.ContentType = "application/json; charset=utf-8";
        context.Response.StatusCode = statusCode;

        var jsonResponse = JsonSerializer.Serialize(payload, _jsonOptions);

        await context.Response.WriteAsync(jsonResponse);
    }
}
