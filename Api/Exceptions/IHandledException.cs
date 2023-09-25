using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace LonelyVale.Api.Exceptions;

public interface IHandledException
{
    Task HandleException(HttpContext context, ILogger logger, IOptions<JsonOptions> jsonOptions);
}