using LonelyVale.Api.Exceptions;
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
    [HttpGet("realms/{id}", Name = "GetRealm")]
    public async Task<GetRealmResponse> Get([FromRoute] GetRealmRequest request)
    {
        var realm = await _repository.GetRealm(request.Id);
        return new GetRealmResponse(
            realm.Id ?? throw new CodedHttpException("Unable to find a realm that has that id."),
            realm.Name,
            realm.Auth0OrgId
        );
    }

    [Authorize("GetRealms")]
    [HttpGet("realms", Name = "GetRealms")]
    public async Task<IEnumerable<GetRealmResponse>> Get([FromQuery] GetRealmsRequest request)
    {
        var user = await _userResolver.ResolveUserForRequest(request.UserId, request.Auth0UserId);
        var realms = await _repository.GetRealmsForUser(user);
        return realms.Select(realm => new GetRealmResponse(
            realm.Id ?? throw new CodedHttpException("A realm in that list is broken and doesn't have an id."),
            realm.Name,
            realm.Auth0OrgId
        )).ToList();
    }
}