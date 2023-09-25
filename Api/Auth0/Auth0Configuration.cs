using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Api.Auth0;

public class Auth0Configuration
{
    public const string SECTION_KEY = "Auth0";
    
    [Required]
    public string Tenant { get; init; } = string.Empty;
}