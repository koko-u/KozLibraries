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
            .SelectMany(t =>
            {
                var attr = t.GetCustomAttribute<AutoRegisterServiceAttribute>(inherit: true);
                var serviceTypes = ResolveServiceTypes(t, attr);
                return serviceTypes
                    .Distinct()
                    .Select(serviceType =>
                    {
                        var lifetime = attr?.Lifetime ?? ServiceLifetime.Scoped;
                        return (
                            serviceType: serviceType,
                            implementationType: t,
                            lifetime: lifetime
                        );
                    });
            });

        foreach (var (serviceType, implementationType, lifetime) in srvTypes)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Scoped:
                    services.AddScoped(serviceType, implementationType);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(serviceType, implementationType);
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient(serviceType, implementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }

            onRegistered?.Invoke(serviceType, lifetime);
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

    private static IEnumerable<Type> ResolveServiceTypes(
        Type implementationType,
        AutoRegisterServiceAttribute? attr
    )
    {
        if (attr is null)
        {
            yield break;
        }

        if (attr.ServiceTypes.Count > 0)
        {
            // AutoRegisterService にサービスタイプが指定されている
            foreach (var serviceType in attr.ServiceTypes)
            {
                if (!serviceType.IsAssignableTo(implementationType))
                {
                    throw new InvalidOperationException(
                        $"Service type {serviceType.Name} is not assignable to implementation type {implementationType.Name}"
                    );
                }

                yield return serviceType;
            }
        }
        else
        {
            // 指定がない場合はインターフェースを探す
            var interfaceName = $"I{implementationType.Name}";
            var interfaceType = implementationType
                .GetInterfaces()
                .FirstOrDefault(i => i.Name == interfaceName);
            if (interfaceType != null)
            {
                // インターフェイスがあればそれが serviceType
                yield return interfaceType;
            }
            else
            {
                // なければ実装クラスが serviceType
                yield return implementationType;
            }
        }

        if (attr.RegisterSelf)
        {
            // 自分自身も登録する
            yield return implementationType;
        }
    }
}
