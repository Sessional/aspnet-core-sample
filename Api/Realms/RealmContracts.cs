using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LonelyVale.Api.Realms;

public record GetRealmResponse(long Id, string Name, string Auth0OrgId);

public record GetRealmRequest(
    [Required] [FromRoute] long Id
);

public record GetRealmsRequest(string? Auth0UserId, long? UserId, long Offset = 0, long Size = 10);