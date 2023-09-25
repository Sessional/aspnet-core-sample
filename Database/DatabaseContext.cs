using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace LonelyVale.Database;

public class DatabaseContext
{
    private readonly IOptions<DatabaseConfiguration> _databaseConnections;

    public DatabaseContext(IOptions<DatabaseConfiguration> databaseConnections)
    {
        _databaseConnections = databaseConnections;
    }

    public IDbConnection GetConnection(string name)
    {
        if (_databaseConnections.Value.ConnectionStrings.TryGetValue(name, out var connectionString))
            return new NpgsqlConnection(connectionString);

        throw new DatabaseNotFoundException($"Unable to find database with name {name}.");
    }
}