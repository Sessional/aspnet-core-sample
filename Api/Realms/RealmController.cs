using LonelyVale.Api.Exceptions;
using LonelyVale.Api.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LonelyVale.Api.Realms;

[ApiController]
public class RealmController : ControllerBase
{
    private readonly RealmRepository _repository;
    private readonly UserService _userService;
    private readonly UserResolver _userResolver;

    public RealmController(
        RealmRepository repository,
        UserService userService,
        UserResolver userResolver
    )
    {
        _repository = repository;
        _userService = userService;
        _userResolver = userResolver;
    }

    [Authorize(RealmPolicies.DescribeRealm)]
    [HttpGet("realms/{id}", Name = "DescribeRealm")]
    public async Task<DescribeRealmResponse> Get([FromRoute] DescribeRealmRequest request)
    {
        var realm = await _repository.GetRealm(request.Id);
        return new DescribeRealmResponse(
            realm.Id ?? throw new CodedHttpException("Unable to find a realm that has that id."),
            realm.Name,
            realm.Auth0OrgId
        );
    }

    [Authorize(RealmPolicies.ListRealms)]
    [HttpGet("realms", Name = "ListRealms")]
    public async Task<ListRealmsResponse> Get([FromQuery] ListRealmsRequest request)
    {
        var user = await _userService.GetUser(UserIdentity.FromFirstValue(request.UserId,
            request.Auth0UserId, _userResolver.CurrentCallerSub));
        var realms = await _repository.GetRealmsForUser(user);
        return new ListRealmsResponse(
            realms.Select(realm => new DescribeRealmResponse(
                realm.Id ?? throw new CodedHttpException("A realm in that list is broken and doesn't have an id."),
                realm.Name,
                realm.Auth0OrgId
            )).ToList()
        );
    }
}