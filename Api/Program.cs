using System.Text.Json;
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

        builder.Services.AddAuthentication("DefaultAuth0Tenant")
            .AddJwtBearer("DefaultAuth0Tenant", options =>
            {
                options.Authority = "https://lonelyvale.us.auth0.com/";
                options.Audience = "http://lonelyvale.com";
                options.MapInboundClaims = false;
            });

        builder.Services.AddAuthorization(options => { options.AddRealmAuthorization(); });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseExceptionHandler(
            app.Services.GetService<ILoggerFactory>()!,
            app.Services.GetService<IOptions<JsonOptions>>()!
        );

        app.UseHeaderBasedTenancy();

        app.MapControllers();

        app.Run();
    }
}