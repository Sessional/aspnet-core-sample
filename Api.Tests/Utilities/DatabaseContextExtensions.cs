using Dapper;
using LonelyVale.Database;

namespace LonelyVale.Api.Tests.Utilities;

public static class DatabaseContextExtensions
{
    public static async Task ClearAllTables(this DatabaseContext context)
    {
        using var connection = context.GetConnection("Primary");

        var tableNames = await connection.QueryAsync<string>("""
                                                             SELECT table_name
                                                             FROM information_schema.tables
                                                             WHERE table_schema = 'public'
                                                             AND table_type = 'BASE TABLE';
                                                             """);
        await ClearTables(context, tableNames.ToArray());
    }

    public static async Task ClearTables(this DatabaseContext context, params string[] tableNames)
    {
        using var connection = context.GetConnection("Primary");

        var tasks = tableNames.Select(x => connection.ExecuteAsync($"TRUNCATE {x} RESTART IDENTITY CASCADE;")).ToList();
        Task.WaitAll(tasks.ToArray());
    }
}