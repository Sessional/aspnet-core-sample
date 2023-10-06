using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace LonelyVale.Api.Realms;

public record DescribeRealmResponse(long Id, string Name, string Auth0OrgId);

public record DescribeRealmRequest(
    [Required] [FromRoute] long Id
);

public record ListRealmsRequest(string? Auth0UserId, long? UserId, long Offset = 0, long Size = 10);

public record ListRealmsResponse(List<DescribeRealmResponse> Realms);