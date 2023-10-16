namespace LonelyVale.Api.Users;

public static class UsersExtensions
{
    public static IServiceCollection AddUsers(this IServiceCollection services)
    {
        services
            .AddSingleton<UserRepository>()
            .AddSingleton<UserService>()
            .AddSingleton<UserResolver>();
        return services;
    }
}