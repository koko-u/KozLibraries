using Microsoft.Extensions.DependencyInjection;

namespace AutoRegisterAnnotation;

public readonly record struct ServiceTypePair(
    Type ServiceType,
    Type ImplementationType,
    ServiceLifetime Lifetime
);
