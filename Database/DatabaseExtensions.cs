namespace LonelyVale.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabaseContext(this IServiceCollection services)
    {
        services.AddOptions<DatabaseConfiguration>()
            .BindConfiguration(DatabaseConfiguration.SECTION_KEY)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddSingleton<DatabaseContext>();
        return services;
    }
}