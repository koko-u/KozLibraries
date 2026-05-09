using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoRegisterAnnotation;

/// <summary>
/// Annotation for classes that should be automatically registered as services.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
[MeansImplicitUse(
    ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature,
    ImplicitUseTargetFlags.Itself
)]
public sealed class AutoRegisterServiceAttribute : Attribute
{
    /// <summary>
    /// Lifetime of the registered service.
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
}
