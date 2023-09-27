using System.Net;
using Dapper;
using LonelyVale.Api.Exceptions;
using LonelyVale.Database;

namespace LonelyVale.Api.Users;

public class UserRepository
{
    private readonly DatabaseContext _databaseContext;

    public UserRepository(DatabaseContext context)
    {
        _databaseContext = context;
    }

    public async Task CreateUser(UserEntity user)
    {
        using var connection = _databaseContext.GetConnection("Primary");
        await connection.ExecuteAsync("""
                                      INSERT INTO users (id, auth0_id) VALUES (@userId,@auth0Id)
                                      """, new
        {
            userId = user.Id,
            auth0Id = user.Auth0Id
        });
    }

    public async Task<UserEntity?> GetUserOrNull(long id)
    {
        using var connection = _databaseContext.GetConnection("Primary");
        var user = await connection.QueryAsync<UserEntity>(
            "SELECT id as Id, auth0_id as Auth0Id from public.users WHERE id=@userId", new
            {
                userId = id
            });

        return user.SingleOrDefault();
    }

    public async Task<UserEntity?> GetUserOrNullByAuth0Id(string auth0Id)
    {
        using var connection = _databaseContext.GetConnection("Primary");
        var user = await connection.QueryAsync<UserEntity>(
            "SELECT id as Id, auth0_id as Auth0Id from public.users WHERE auth0_id=@auth0Id", new
            {
                auth0Id = auth0Id
            });

        return user.SingleOrDefault();
    }

    public async Task<UserEntity> GetUserByAuth0Id(string auth0Id)
    {
        return await GetUserOrNullByAuth0Id(auth0Id) ??
               throw new CodedHttpException("Unable to find user with that id.", HttpStatusCode.NotFound);
    }

    public async Task<UserEntity> GetUser(long id)
    {
        return await GetUserOrNull(id) ??
               throw new CodedHttpException("Unable to find user with that id.", HttpStatusCode.NotFound);
    }
}