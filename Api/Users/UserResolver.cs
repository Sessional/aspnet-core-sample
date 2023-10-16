using System.Security.Claims;

namespace LonelyVale.Api.Users;

public class UserResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? ResolveCurrentUserClaim(string claim)
    {
        return _httpContextAccessor.HttpContext?.User!.FindFirstValue(claim);
    }

    public string? CurrentCallerSub => ResolveCurrentUserClaim("sub");
}