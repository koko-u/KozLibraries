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
    /// <param name="type">type in the assembly</param>
    /// <param name="onRegistered">registered action for each service registration</param>
    /// <returns></returns>
    public static IServiceCollection AddAutoRegisterServices(
        this IServiceCollection services,
        Type type,
        Action<Type, ServiceLifetime>? onRegistered = null
    )
    {
        var srvTypes = type
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
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(service);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(service);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(service);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }

            onRegistered?.Invoke(service, lifetime);
        }
        return services;
    }

    /// <summary>
    /// Registers services based on the AutoRegisterServiceAttribute.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="onRegistered">registered action for each service registration</param>
    /// <typeparam name="T">Some class in the assembly</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IServiceCollection AddAutoRegisterServices<T>(
        this IServiceCollection services,
        Action<Type, ServiceLifetime>? onRegistered = null
    )
        where T : class
    {
        return services.AddAutoRegisterServices(typeof(T), onRegistered);
    }
}
