using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace LonelyVale.Api.Exceptions;

public class CodedHttpException : Exception, IHandledException
{
    private HttpStatusCode StatusCode { get; }
    private string? ExtendedMessage { get; }

    public CodedHttpException(string? message) : base(message)
    {
        StatusCode = HttpStatusCode.InternalServerError;
    }

    public CodedHttpException(string? message, HttpStatusCode statusCode) : this(message)
    {
        StatusCode = statusCode;
    }

    public CodedHttpException(string? message, string? extendedMessage, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = HttpStatusCode.InternalServerError;
        ExtendedMessage = extendedMessage;
    }

    public Task HandleException(HttpContext context, ILogger logger, IOptions<JsonOptions> jsonOptions)
    {
        StringBuilder builder = new("A caught http exception occurred.");
        builder.Append($" Status code is {StatusCode}");
        if (ExtendedMessage != null) builder.Append($" {ExtendedMessage}");
        var statusCode = (int)StatusCode;
        switch (statusCode)
        {
            case >= 400:
                logger.LogWarning(this, builder.ToString());
                break;
            default:
                logger.LogError(this, builder.ToString());
                break;
        }

        var json = JsonSerializer.Serialize(
            new { Message },
            jsonOptions?.Value.SerializerOptions);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        return context.Response.WriteAsync(json);
    }
}