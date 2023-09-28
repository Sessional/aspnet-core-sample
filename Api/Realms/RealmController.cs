using LonelyVale.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LonelyVale.Api.Realms;

[ApiController]
public class RealmController : ControllerBase
{
    private readonly ILogger<RealmController> _logger;
    private readonly RealmRepository _repository;
    private readonly UserResolver _userResolver;

    public RealmController(ILogger<RealmController> logger,
        RealmRepository repository,
        UserResolver userResolver)
    {
        _logger = logger;
        _repository = repository;
        _userResolver = userResolver;
    }

    [Authorize("DescribeRealm")]
    [HttpGet("realms/{Id}", Name = "GetRealm")]
    public async Task<GetRealmResponse> Get([FromQuery] GetRealmRequest request)
    {
        var realm = await _repository.GetRealm(request.Id);
        return new GetRealmResponse()
        {
            Id = realm.Id,
            Name = realm.Name,
            Auth0OrgId = realm.Auth0OrgId
        };
    }

    [Authorize("GetRealms")]
    [HttpGet("realms", Name = "GetRealms")]
    public async Task<IEnumerable<GetRealmResponse>> Get([FromQuery] GetRealmsRequest request)
    {
        var user = await _userResolver.ResolveUserForRequest(request.UserId, request.Auth0UserId);
        var realms = await _repository.GetRealmsForUser(user);
        return realms.Select(realm => new GetRealmResponse()
        {
            Id = realm.Id,
            Name = realm.Name,
            Auth0OrgId = realm.Auth0OrgId
        }).ToList();
    }
}