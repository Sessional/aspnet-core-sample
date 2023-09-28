using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Api.Users;

public record GetUserResponse
{
    public long Id { get; set; }
    public required string Auth0Id { get; set; }
}

public record GetUserRequest
{
    public long? Id { get; set; }
    public string? Auth0Id { get; set; }
}