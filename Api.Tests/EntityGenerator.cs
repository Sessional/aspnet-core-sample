using LonelyVale.Api.Users;

namespace LonelyVale.Api.Tests;

public static class EntityGenerator
{
    public static UserEntity CreateUser()
    {
        var entity = new UserEntity(
            -1,
            "auth0|12345");

        return entity;
    }
}