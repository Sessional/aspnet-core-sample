using System.Net;
using System.Security.Claims;
using Dapper;
using LonelyVale.Api.Exceptions;
using LonelyVale.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LonelyVale.Api.Users;

[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly DatabaseContext _databaseContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserRepository _repository;

    public UserController(ILogger<UserController> logger, DatabaseContext context,
        IHttpContextAccessor httpContextAccessor, UserRepository repository)
    {
        _logger = logger;
        _databaseContext = context;
        _httpContextAccessor = httpContextAccessor;
        _repository = repository;
    }

    [HttpGet("users/{userId}", Name = "GetUserById")]
    public async Task<GetUserResponse> GetUserById(long userId)
    {
        using (var connection = _databaseContext.GetConnection("Primary"))
        {
            var user = await connection.QueryAsync<UserEntity>(
                "SELECT id as Id, auth0_id as Auth0Id from public.users WHERE id=@userId", new
                {
                    userId = userId
                });
            return new GetUserResponse()
            {
                Id = user.Single().Id,
                Auth0Id = user.Single().Auth0Id
            };
        }
    }

    [HttpGet("users", Name = "GetUser")]
    public async Task<GetUserResponse> GetUser([FromQuery] GetUserRequest request)
    {
        var caller = _httpContextAccessor.HttpContext?.User!;
        var callerAuth0UserId = caller.FindFirstValue("sub");
        var auth0UserIdToGet = request.Auth0Id ?? callerAuth0UserId;

        if (!request.Id.HasValue && auth0UserIdToGet.IsNullOrEmpty())
            throw new CodedHttpException(
                "Unable to determine a user to get. Please supply either a userId or an auth0UserId",
                HttpStatusCode.BadRequest
            );

        using var connection = _databaseContext.GetConnection("Primary");
        if (request.Id.HasValue)
        {
            var user = await _repository.GetUser(request.Id.Value);
            return new GetUserResponse
            {
                Id = user.Id,
                Auth0Id = user.Auth0Id
            };
        }


        if (!auth0UserIdToGet.IsNullOrEmpty())
        {
            var user = await _repository.GetUserByAuth0Id(auth0UserIdToGet!);
            return new GetUserResponse
            {
                Id = user.Id,
                Auth0Id = user.Auth0Id
            };
        }

        throw new CodedHttpException("Unable to resolve a user to retrieve.", HttpStatusCode.BadRequest);
    }
}