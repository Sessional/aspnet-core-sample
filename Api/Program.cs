using System.Security.Claims;
using System.Threading.RateLimiting;
using LonelyVale.Api.Auth0;
using LonelyVale.Api.Exceptions;
using LonelyVale.Api.Logging;
using LonelyVale.Api.Realms;
using LonelyVale.Api.Users;
using LonelyVale.Database;
using LonelyVale.Tenancy;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

namespace LonelyVale.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureLogging();

        // Add services to the container.
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(o => o.DescribeAllParametersInCamelCase());

        builder.Services.AddAuth0();
        builder.Services.AddDatabaseContext();
        builder.Services.AddTenancyComponents();

        builder.Services.AddUsers();
        builder.Services.AddRealms();

        builder.Services.AddAuthentication("DefaultAuth0Issuer")
            .AddJwtBearer("DefaultAuth0Issuer", options =>
            {
                options.Authority = "https://lonelyvale.us.auth0.com/";
                options.Audience = "http://lonelyvale.com";
                options.MapInboundClaims = false;
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAuthentication", policyBuilder =>
                policyBuilder.RequireAuthenticatedUser()
                    .Build());
            options.DefaultPolicy = options.GetPolicy("RequireAuthentication")!;
            options.AddRealmAuthorization();
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    httpContext.User.FindFirstValue("sub") ?? httpContext.Request.Headers.Host.ToString(),
                    partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 10,
                        QueueLimit = 0,
                        Window = TimeSpan.FromSeconds(10)
                    }));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler(
            app.Services.GetService<ILoggerFactory>()!,
            app.Services.GetService<IOptions<JsonOptions>>()!
        );

        app.UseHeaderBasedTenancy();

        app.MapControllers()
            .RequireAuthorization();

        app.Run();
    }
}