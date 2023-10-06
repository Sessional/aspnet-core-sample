using LonelyVale.Api.Realms;
using LonelyVale.Api.Users;

namespace LonelyVale.Api.Tests;

public static class EntityGenerator
{
    public static UserEntity CreateUser()
    {
        return new UserEntity(-1, "auth0|12345");
    }

    public static RealmEntity CreateRealm()
    {
        return new RealmEntity(1, "randomName", "org_54321", true, "tenant_1");
    }
}