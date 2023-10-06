using System.Net;
using System.Security.Claims;
using Dapper;
using LonelyVale.Api.Exceptions;
using LonelyVale.Database;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpGet("users/{userId}", Name = "GetUserById")]
    public async Task<GetUserResponse> GetUserById(long userId)
    {
        var user = await _repository.GetUserOrNull(userId) ??
                   throw new CodedHttpException("Unable to find a user with that id.");

        return new GetUserResponse(
            user.Id ?? throw new CodedHttpException("Error finding a user with that ID."),
            user.Auth0Id
        );
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
            return new GetUserResponse(
                user.Id ?? throw new CodedHttpException("Unable to find a user with an id for some reason."),
                user.Auth0Id
            );
        }


        if (!auth0UserIdToGet.IsNullOrEmpty())
        {
            var user = await _repository.GetUserByAuth0Id(auth0UserIdToGet!);
            return new GetUserResponse(
                user.Id ?? throw new CodedHttpException("Unable to find a user with an ID for some reason."),
                user.Auth0Id
            );
        }

        throw new CodedHttpException("Unable to resolve a user to retrieve.", HttpStatusCode.BadRequest);
    }

    [HttpPost("users", Name = "CreateUser")]
    public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
    {
        var userId = await _repository.CreateUser(new UserEntity(null, request.Auth0Id));

        return new CreateUserResponse(
            userId,
            request.Auth0Id
        );
    }
}