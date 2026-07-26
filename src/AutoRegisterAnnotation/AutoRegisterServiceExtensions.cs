using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace AutoRegisterAnnotation;

/// <summary>
/// Extension methods for auto-registering services based on attributes.
/// </summary>
public static class AutoRegisterServiceExtensions
{
    /// <summary>
    /// Scans the assembly containing <paramref name="type"/> and registers concrete classes
    /// directly annotated with <see cref="AutoRegisterServiceAttribute"/>.
    /// Inherited attributes are not considered.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="type">type in the assembly</param>
    /// <param name="onRegistered">
    /// Action invoked after each service registration. The callback must not throw an exception;
    /// otherwise, services registered before the exception will remain in the collection.
    /// </param>
    /// <returns></returns>
    public static IServiceCollection AddAutoRegisterServices(
        this IServiceCollection services,
        Type type,
        Action<ServiceTypePair>? onRegistered = null
    )
    {
        var srvTypes = type
            .Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetCustomAttribute<AutoRegisterServiceAttribute>(inherit: false) is not null)
            .SelectMany(t =>
            {
                var attr = t.GetCustomAttribute<AutoRegisterServiceAttribute>(inherit: false);
                var serviceTypes = ResolveServiceTypes(t, attr);
                return serviceTypes
                    .Distinct()
                    .Select(serviceType =>
                    {
                        var lifetime = attr?.Lifetime ?? ServiceLifetime.Scoped;
                        return new ServiceTypePair(serviceType, t, lifetime);
                    });
            })
            .ToList();

        // check Lifetime
        foreach (var srvType in srvTypes)
        {
            if (!Enum.IsDefined(typeof(ServiceLifetime), srvType.Lifetime))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(srvType.Lifetime),
                    srvType.Lifetime,
                    "Service type lifetime is not defined"
                );
            }
        }

        // register DI
        foreach (var srvType in srvTypes)
        {
            var serviceDescriptor = new ServiceDescriptor(
                srvType.ServiceType,
                srvType.ImplementationType,
                srvType.Lifetime
            );
            services.Add(serviceDescriptor);

            onRegistered?.Invoke(srvType);
        }
        return services;
    }

    /// <summary>
    /// Scans the assembly containing <typeparamref name="T"/> and registers concrete classes
    /// directly annotated with <see cref="AutoRegisterServiceAttribute"/>.
    /// Inherited attributes are not considered.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="onRegistered">
    /// Action invoked after each service registration. The callback must not throw an exception;
    /// otherwise, services registered before the exception will remain in the collection.
    /// </param>
    /// <typeparam name="T">Some class in the assembly</typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IServiceCollection AddAutoRegisterServices<T>(
        this IServiceCollection services,
        Action<ServiceTypePair>? onRegistered = null
    )
        where T : class
    {
        return services.AddAutoRegisterServices(typeof(T), onRegistered);
    }

    private static IEnumerable<Type> ResolveServiceTypes(Type implementationType, AutoRegisterServiceAttribute? attr)
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
                if (!serviceType.IsAssignableFrom(implementationType))
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
            var interfaceTypes = implementationType.GetInterfaces().Where(i => i.Name == interfaceName).ToList();
            if (interfaceTypes.Count > 1)
            {
                // 規約に一致するインターフェイスが複数ある場合は、曖昧なので失敗させる
                throw new InvalidOperationException("Ambiguous service type found");
            }

            if (interfaceTypes.Count == 1)
            {
                // 規約に一致するインターフェイスが一つに決まれば、それを serviceType とする
                yield return interfaceTypes[0];
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
