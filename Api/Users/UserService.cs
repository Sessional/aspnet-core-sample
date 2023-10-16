using System.Net;
using System.Transactions;
using LonelyVale.Api.Exceptions;
using LonelyVale.Database;

namespace LonelyVale.Api.Users;

public enum UserIdentityType
{
    USER_ID,
    AUTH0_ID
}

public interface IUserIdentity
{
    public UserIdentityType Type { get; }
}

public static class UserIdentity
{
    public static IUserIdentity FromAuth0Id(string auth0Id)
    {
        return new Auth0UserIdentity(auth0Id, UserIdentityType.AUTH0_ID);
    }

    public static IUserIdentity FromId(long id)
    {
        return new UserIdIdentity(id, UserIdentityType.USER_ID);
    }

    public static IUserIdentity FromFirstValue(params object?[] values)
    {
        foreach (var value in values)
        {
            if (value == null) continue;
            return value switch
            {
                long l => FromId(l),
                string s => FromAuth0Id(s),
                _ => throw new CodedHttpException(
                    $"Unable to retrieve a value for the specified type. {value.GetType()}",
                    HttpStatusCode.BadRequest)
            };
        }

        throw new CodedHttpException(
            "Unable to determine a user to get. Specify at least one value.",
            HttpStatusCode.BadRequest);
    }
}

public abstract class UserIdentity<T> : IUserIdentity
{
    public UserIdentityType Type { get; private set; }

    public T Identifier { get; private set; }

    public UserIdentity(T id, UserIdentityType identityType)
    {
        Identifier = id;
        Type = identityType;
    }
}

public class Auth0UserIdentity : UserIdentity<string>
{
    public Auth0UserIdentity(string id, UserIdentityType identityType)
        : base(id, identityType)
    {
    }
}

public class UserIdIdentity : UserIdentity<long>
{
    public UserIdIdentity(long id, UserIdentityType identityType)
        : base(id, identityType)
    {
    }
}

public class UserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly UserRepository _repository;

    public UserService(DatabaseContext databaseContext, UserRepository repository)
    {
        _databaseContext = databaseContext;
        _repository = repository;
    }

    public async Task<UserEntity> GetUserById(long userId)
    {
        return await _repository.GetUser(userId);
    }

    public async Task<UserEntity> GetUserByAuth0Id(string auth0Id)
    {
        return await _repository.GetUserByAuth0Id(auth0Id);
    }

    public async Task<UserEntity> GetUser(IUserIdentity identity)
    {
        return identity switch
        {
            Auth0UserIdentity auth0 => await GetUserByAuth0Id(auth0.Identifier),
            UserIdIdentity id => await GetUserById(id.Identifier)
        };
    }
}