using LonelyVale.Api.Realms;
using LonelyVale.Api.Tests.Fixtures;
using LonelyVale.Api.Users;

namespace LonelyVale.Api.Tests.Utilities;

public static class EntityGenerator
{
    public static UserEntity CreateUser()
    {
        return new UserEntity(null, Arbitrary.ArbitraryAuth0Id());
    }

    public static RealmEntity CreateRealm()
    {
        return new RealmEntity(null, Arbitrary.ArbitraryString(), Arbitrary.ArbitraryOrgId(), true,
            "tenant_1");
    }
}