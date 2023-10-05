using LonelyVale.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace LonelyVale.Api.Tests;

public static class TestExtensions
{
    public static WebApplicationFactory<Program> ConfigureStandardTest(this WebApplicationFactory<Program> factory,
        DatabaseFixture databaseFixture)
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(o =>
                {
                    o.AddFilter("Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware", LogLevel.None);
                    o.AddFilter("LonelyVale.Api.Exceptions.ExceptionHandler", LogLevel.None);
                    o.AddFilter("Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", LogLevel.None);
                    o.AddFilter("TestContainers", LogLevel.None);
                }
            );
            builder.ConfigureTestServices(services =>
            {
                services.Configure<DatabaseConfiguration>(opts =>
                    opts.ConnectionStrings = new Dictionary<string, string>()
                    {
                        {
                            "Primary",
                            databaseFixture.PostgresContainer.GetConnectionString()
                        }
                    }
                );
                services.Configure<JwtBearerOptions>("DefaultAuth0Issuer", options =>
                {
                    var config = new OpenIdConnectConfiguration()
                    {
                        Issuer = MockJwtTokens.Issuer
                    };

                    options.Audience = MockJwtTokens.Audience;
                    config.SigningKeys.Add(MockJwtTokens.SecurityKey);
                    options.Configuration = config;
                });
            });
        });
    }
}