using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Api.Users;

public record GetUserResponse(long Id, string Auth0Id);

public record GetUserRequest(long? Id, string? Auth0Id);

public record CreateUserRequest(string Auth0Id);

public record CreateUserResponse(long Id, string Auth0Id);