using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Api.Users;

public record GetUserResponse
{
    public long Id { get; set; }
    public string Auth0Id { get; set; }
}

public record GetUserRequest
{
    public long? id { get; set; }
    public string? auth0Id { get; set; }
}