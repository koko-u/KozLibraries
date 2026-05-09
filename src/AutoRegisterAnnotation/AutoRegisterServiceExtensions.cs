using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoRegisterAnnotation;

/// <summary>
/// Extension methods for auto-registering services based on attributes.
/// </summary>
public static class AutoRegisterServiceExtensions
{
    /// <summary>
    /// Registers services based on the AutoRegisterServiceAttribute.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IServiceCollection AddAutoRegisterServices<T>(this IServiceCollection services)
        where T : class
    {
        var srvTypes = typeof(T)
            .Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t =>
                t.GetCustomAttribute<AutoRegisterServiceAttribute>(inherit: true) is not null
            )
            .Select(t =>
            {
                var attr = t.GetCustomAttribute<AutoRegisterServiceAttribute>(inherit: true);
                return (service: t, lifetime: attr?.Lifetime ?? ServiceLifetime.Scoped);
            });

        foreach (var (service, lifetime) in srvTypes)
        {
            return lifetime switch
            {
                ServiceLifetime.Scoped => services.AddScoped(service),
                ServiceLifetime.Singleton => services.AddSingleton(service),
                ServiceLifetime.Transient => services.AddTransient(service),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null),
            };
        }
        return services;
    }
}
