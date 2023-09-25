using System.Security.Claims;
using LonelyVale.Api.Exceptions;
using Microsoft.IdentityModel.Tokens;

namespace LonelyVale.Api.Users;

public class UserResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserRepository _repository;

    public UserResolver(IHttpContextAccessor httpContextAccessor, UserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _repository = userRepository;
    }

    private string? GetAuth0UserIdOrNull()
    {
        var caller = _httpContextAccessor.HttpContext?.User!;
        var callerAuth0UserId = caller.FindFirstValue("sub");
        return callerAuth0UserId;
    }

    public async Task<UserEntity> ResolveUserForRequest(long? userId, string? auth0Id)
    {
        if (userId.HasValue) return await _repository.GetUser(userId.Value);
        if (!auth0Id.IsNullOrEmpty()) return await _repository.GetUserByAuth0Id(auth0Id);
        var callerAuth0Id = GetAuth0UserIdOrNull() ?? throw new CodedHttpException("Unable to resolve a user.");
        return await _repository.GetUserByAuth0Id(callerAuth0Id);
    }
}