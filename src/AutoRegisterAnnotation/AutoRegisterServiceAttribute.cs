using JetBrains.Annotations;

namespace AutoRegisterAnnotation;

/// <summary>
/// Annotation for classes that should be automatically registered as services.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
[MeansImplicitUse(
    ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature,
    ImplicitUseTargetFlags.Itself
)]
public sealed class AutoRegisterServiceAttribute : Attribute;
