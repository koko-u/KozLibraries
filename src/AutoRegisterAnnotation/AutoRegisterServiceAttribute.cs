using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace AutoRegisterAnnotation;

/// <summary>
/// Annotation for classes that should be automatically registered as services.
///
/// This attribute is not inherited. Each concrete class to be registered must be
/// annotated directly.
///
/// 1. AutoRegisterService(typeof(...)) と指定された場合は、typeof に指定した型を DI に登録する
/// 2. AutoRegisterService とだけ指定された場合は、クラスに I 接頭辞を付けたインターフェイスがあれば、そのインターフェイスの実装を DI に登録する
///    AutoRegisterService と指定されたが、接頭辞 I を付けたインターフェイスがなければ、クラス自体を DI に登録する
/// 3. AutoRegisterService(RegisterSelf = true) と指定された場合は、クラス自体を DI に登録する
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature, ImplicitUseTargetFlags.Itself)]
public sealed class AutoRegisterServiceAttribute : Attribute
{
    /// <summary>
    /// コンストラクタです。DIに登録する型を明示します
    /// </summary>
    /// <param name="types"></param>
    public AutoRegisterServiceAttribute(params Type[] types)
    {
        ServiceTypes = types;
    }

    /// <summary>
    /// Lifetime of the registered service.
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;

    public IReadOnlyList<Type> ServiceTypes { get; }

    public bool RegisterSelf { get; set; } = false;
}
