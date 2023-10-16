using LonelyVale.Api.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LonelyVale.Api.Users;

[ApiController]
public class UserController : ControllerBase
{
    private readonly UserRepository _repository;
    private readonly UserService _userService;
    private readonly UserResolver _userResolver;

    public UserController(UserService userService, UserResolver userResolver,
        UserRepository repository)
    {
        _repository = repository;
        _userService = userService;
        _userResolver = userResolver;
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
        var userIdentity = UserIdentity.FromFirstValue(
            request.Id,
            request.Auth0Id,
            _userResolver.CurrentCallerSub);
        var user = await _userService.GetUser(userIdentity);
        return new GetUserResponse(
            user.Id ?? throw new CodedHttpException("Unable to find a user with an id for some reason."),
            user.Auth0Id
        );
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