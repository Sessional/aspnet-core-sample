using System.Net;
using Dapper;
using LonelyVale.Api.Exceptions;
using LonelyVale.Api.Users;
using LonelyVale.Database;

namespace LonelyVale.Api.Realms;

public class RealmRepository
{
    private readonly DatabaseContext _databaseContext;

    public RealmRepository(DatabaseContext context)
    {
        _databaseContext = context;
    }

    public async Task<RealmEntity?> GetRealmOrNull(long id)
    {
        using var connection = _databaseContext.GetConnection("Primary");
        var realm = await connection.QueryAsync<RealmEntity>(
            """
            SELECT id as Id,
                   realm_name as Name,
                   auth0_org_id as Auth0OrgId,
                   is_public as IsPublic
            from public.realms
            WHERE id=@realmId
            """, new
            {
                realmId = id
            });

        return realm.SingleOrDefault();
    }

    public async Task<RealmEntity?> GetRealmByAuth0OrgIdOrNull(string orgId)
    {
        using var connection = _databaseContext.GetConnection("Primary");
        var realm = await connection.QueryAsync<RealmEntity>(
            """
            SELECT id as Id,
                realm_name as Name,
                auth0_org_id as Auth0OrgId,
                is_public as IsPublic
            from public.realms
            WHERE auth0_org_id=@orgId
            """,
            new
            {
                orgId = orgId
            });

        return realm.SingleOrDefault();
    }

    public async Task<RealmEntity> GetRealmByAuth0OrgId(string orgId)
    {
        return await GetRealmByAuth0OrgIdOrNull(orgId) ??
               throw new CodedHttpException("Unable to realm user with that id.", HttpStatusCode.NotFound);
    }

    public async Task<RealmEntity> GetRealm(long id)
    {
        return await GetRealmOrNull(id) ??
               throw new CodedHttpException("Unable to find realm with that id.", HttpStatusCode.NotFound);
    }

    public async Task<IEnumerable<RealmEntity>> GetRealmsForUser(UserEntity user)
    {
        using var connection = _databaseContext.GetConnection("Primary");
        var realms = await connection.QueryAsync<RealmEntity>(
            """
            SELECT realms.id as Id,
                   realms.realm_name as Name,
                   realms.auth0_org_id as Auth0OrgId,
                   realms.is_public as IsPublic
                   from realm_membership
                inner join realms
                    on realm_membership.realm_id = realms.id
                     OR realms.is_public = true
            WHERE realm_membership.user_id = CAST(@userId as BIGINT);
            """,
            new
            {
                userId = user.Id
            });

        return realms;
    }
}