using System.ComponentModel.DataAnnotations;

namespace LonelyVale.Api.Realms;

public class GetRealmResponse
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Auth0OrgId { get; set; }
}

public class GetRealmRequest
{
    [Required] public long Id { get; set; }
}

public class GetRealmsRequest
{
    public string? auth0UserId { get; set; }
    public long? userId { get; set; }

    public long offset { get; set; }
    public long size { get; set; }
}