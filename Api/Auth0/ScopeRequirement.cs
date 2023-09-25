using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace LonelyVale.Api.Auth0;

public class ScopeRequirement : IAuthorizationRequirement
{
    public string RequiredScope { get; }

    public ScopeRequirement(string requiredScope)
    {
        RequiredScope = requiredScope;
    }
}

public class ScopeRequirementHandler : AuthorizationHandler<ScopeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ScopeRequirement requirement)
    {
        var scopes = context.User.FindFirstValue("scope");
        if (scopes.IsNullOrEmpty()) return Task.CompletedTask;

        if (scopes!.Split(" ").Contains(requirement.RequiredScope))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}