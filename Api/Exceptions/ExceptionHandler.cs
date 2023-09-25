using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace LonelyVale.Api.Exceptions;

public class ExceptionHandler
{
    private ILoggerFactory LoggerFactory { get; }
    private ILogger Logger { get; }

    private IOptions<JsonOptions> JsonOptions { get; }

    public RequestDelegate Handler { get; }

    public ExceptionHandler(ILoggerFactory loggerFactory, IOptions<JsonOptions> jsonOptions)
    {
        LoggerFactory = loggerFactory;
        JsonOptions = jsonOptions;
        Logger = loggerFactory.CreateLogger<ExceptionHandler>();
        Handler = context =>
        {
            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();

            switch (exceptionHandlerPathFeature.Error)
            {
                case IHandledException e:
                    return e.HandleException(context, Logger, JsonOptions);
                default:
                    Logger.LogError(exceptionHandlerPathFeature.Error,
                        $"An unhandled error occurred. {exceptionHandlerPathFeature.Error.Message}");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    var json = JsonSerializer.Serialize(
                        new { exceptionHandlerPathFeature.Error.Message },
                        JsonOptions?.Value.SerializerOptions);
                    return context.Response.WriteAsync(json);
            }
        };
    }
}