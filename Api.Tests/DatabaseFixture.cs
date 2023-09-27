using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Testcontainers.PostgreSql;

namespace LonelyVale.Api.Tests;

public class DatabaseFixture : IDisposable
{
    private static DirectoryInfo TryGetSolutionDirectoryInfo(string? currentPath = null)
    {
        var directory = new DirectoryInfo(
            currentPath ?? Directory.GetCurrentDirectory());
        while (directory != null && !directory.GetFiles("*.sln").Any()) directory = directory.Parent;
        return directory!;
    }

    public PostgreSqlContainer PostgresContainer { get; }
    public IContainer AtlasContainer { get; }

    public string JdbcConnectionString
    {
        get
        {
            var connectionString = PostgresContainer.GetConnectionString();
            var parts = connectionString.Split(";");
            var port = parts.First(x => x.StartsWith("Port")).Split("=")[1];
            var database = parts.First(x => x.StartsWith("Database")).Split("=")[1];
            var username = parts.First(x => x.StartsWith("Username")).Split("=")[1];
            var password = parts.First(x => x.StartsWith("Password")).Split("=")[1];
            return $"postgres://{username}:{password}@host.docker.internal:{port}/{database}?sslmode=disable";
        }
    }

    public DatabaseFixture()
    {
        TestcontainersSettings.Logger =
            new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider().CreateLogger("TestContainers");
        PostgresContainer = new PostgreSqlBuilder().Build();
        Task.WaitAll(Task.Run(async () => { await PostgresContainer.StartAsync(); }));
        var directory = TryGetSolutionDirectoryInfo();
        AtlasContainer =
            new ContainerBuilder()
                .WithImage("arigaio/atlas:latest")
                .WithResourceMapping(
                    new DirectoryInfo($"{directory}/Migrations/tenant"),
                    "/migrations")
                .WithCommand(
                    "schema",
                    "apply",
                    "--auto-approve",
                    "--url",
                    JdbcConnectionString,
                    "--to",
                    "file://migrations",
                    "--var",
                    "tenant_id=1",
                    "--schema=tenant_1,public")
                .Build();

        Task.WaitAll(Task.Run(async () =>
        {
            await AtlasContainer.StartAsync();
            await AtlasContainer.GetExitCodeAsync();
        }));
    }

    public void Dispose()
    {
        Task.WaitAll(Task.Run(async () =>
        {
            await PostgresContainer.DisposeAsync();
            await AtlasContainer.DisposeAsync();
        }));
    }
}