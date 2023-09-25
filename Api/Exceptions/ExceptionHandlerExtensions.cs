using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace LonelyVale.Api.Exceptions;

public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandler(
        this IApplicationBuilder builder, ILoggerFactory loggerFactory, IOptions<JsonOptions> jsonOptions)
    {
        var handler = new ExceptionHandler(loggerFactory, jsonOptions);
        builder.UseExceptionHandler(new ExceptionHandlerOptions()
        {
            ExceptionHandler = handler.Handler
        });
        return builder;
    }
}