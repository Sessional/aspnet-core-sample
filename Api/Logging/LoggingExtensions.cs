using System.Text.Json;

namespace LonelyVale.Api.Logging;

public static class LoggingExtensions
{
    public static WebApplicationBuilder ConfigureLogging(
        this WebApplicationBuilder builder)
    {
        var useJson = builder.Configuration.GetValue<bool>("EnableJsonLogging");
        if (useJson)
            builder.Logging.AddJsonConsole(options =>
            {
                options.IncludeScopes = false;
                options.TimestampFormat = "HH:mm:ss ";
                options.JsonWriterOptions = new JsonWriterOptions();
            });
        return builder;
    }
}