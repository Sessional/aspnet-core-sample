using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace LonelyVale.Api.Exceptions;

public class HttpClientException : Exception, IHandledException
{
    public string? HttpResponseBody { get; }
    public HttpStatusCode HttpResponseStatusCode { get; }

    public HttpClientException(string? message, HttpStatusCode httpResponseStatusCode, string? httpResponseBody) :
        base(message)
    {
        HttpResponseBody = httpResponseBody;
        HttpResponseStatusCode = httpResponseStatusCode;
    }

    public Task HandleException(HttpContext context, ILogger logger, IOptions<JsonOptions> jsonOptions)
    {
        var builder = new StringBuilder("The HttpClient experienced an error.");
        builder.Append($" Status: {HttpResponseStatusCode}.");
        if (HttpResponseBody != null) builder.Append($" Response: {HttpResponseBody}");

        logger.LogError(this, builder.ToString());

        var json = JsonSerializer.Serialize(
            new { Message },
            jsonOptions?.Value.SerializerOptions);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        return context.Response.WriteAsync(json);
    }
}