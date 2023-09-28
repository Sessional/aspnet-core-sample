using System.Net;
using LonelyVale.Api.Exceptions;
using LonelyVale.OAuth.Configuration;
using LonelyVale.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LonelyVale.Api.Auth0;

[ApiController]
[Route("auth0")]
public class Auth0Controller
{
    private Auth0ManagementHttpClient Client { get; }
    private IOptions<OAuthClientConfiguration> Config { get; }
    private ITenantIdResolver TenantIdResolver { get; }

    public Auth0Controller(Auth0ManagementHttpClient httpClient, IOptions<OAuthClientConfiguration> configuration,
        ITenantIdResolver tenantIdResolver)
    {
        Client = httpClient;
        Config = configuration;
        TenantIdResolver = tenantIdResolver;
    }


    [HttpGet("/userinfo", Name = "Get User Info")]
    [AllowAnonymous]
    public async Task<Dictionary<string, object>> GetUserInfo(
        [FromQuery] string userId = "auth0|6501eb739371dd2ea8644e2e")
    {
        return await Client.GetUserInfoOrNull(userId) ??
               throw new CodedHttpException("Unable to find a user with that ID", HttpStatusCode.NotFound);
    }

    [HttpGet("/tenant-info", Name = "Get Tenant Into")]
    [AllowAnonymous]
    public string GetTenantInfo()
    {
        return TenantIdResolver.GetTenantId();
    }
}