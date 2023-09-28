using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Api.Realms;

public class GetRealmResponse
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public required string Auth0OrgId { get; set; }
}

public class GetRealmRequest
{
    [Required] public long Id { get; set; }
}

public class GetRealmsRequest
{
    public string? Auth0UserId { get; set; }
    public long? UserId { get; set; }

    public long Offset { get; set; }
    public long Size { get; set; }
}