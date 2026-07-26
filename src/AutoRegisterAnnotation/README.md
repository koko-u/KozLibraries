# KozLibraries.AutoRegisterAnnotation

`KozLibraries.AutoRegisterAnnotation` は、属性を付けたクラスを
`Microsoft.Extensions.DependencyInjection` の DI コンテナーへまとめて登録するための
ライブラリです。

指定したアセンブリだけを走査し、`AutoRegisterServiceAttribute` が付いた具象クラスを
登録します。

## インストール

```sh
dotnet add package KozLibraries.AutoRegisterAnnotation
```

## 基本的な使い方

サービスの実装クラスへ `[AutoRegisterService]` を付けます。

```csharp
using AutoRegisterAnnotation;

public interface IUserService
{
    Task<string> GetNameAsync(int id);
}

[AutoRegisterService]
public sealed class UserService : IUserService
{
    public Task<string> GetNameAsync(int id)
    {
        return Task.FromResult($"User {id}");
    }
}
```

アプリケーションの起動時に、そのクラスが含まれるアセンブリを指定して登録します。

```csharp
builder.Services.AddAutoRegisterServices<UserService>();
```

型を引数で渡すこともできます。

```csharp
builder.Services.AddAutoRegisterServices(typeof(UserService));
```

上の例では、クラス名 `UserService` に `I` を付けた
`IUserService` インターフェイスが自動的に選ばれ、
`IUserService` と `UserService` の組み合わせが登録されます。

## サービス型の決まり方

`AutoRegisterServiceAttribute` の指定方法によって、登録されるサービス型が変わります。

### 規約に基づいて登録する

```csharp
[AutoRegisterService]
public sealed class UserService : IUserService;
```

引数を省略すると、実装クラス名に `I` を付けた名前のインターフェイスを探します。
該当するインターフェイスがなければ、実装クラス自身をサービス型として登録します。

### サービス型を明示する

```csharp
[AutoRegisterService(typeof(IUserService), typeof(IUserReader))]
public sealed class UserService : IUserService, IUserReader;
```

コンストラクターでは複数のサービス型を指定できます。指定できるのは、実装クラスを
代入できるサービス型だけです。実装していないインターフェイスや継承関係のないクラスを
指定すると、登録時に `InvalidOperationException` が発生します。

名前規約に一致する同名のインターフェイスが異なる名前空間に複数ある場合は、
登録先を一意に決められないため `InvalidOperationException` が発生します。その場合は、
上の例のようにサービス型を明示してください。

### 実装クラス自身も登録する

```csharp
[AutoRegisterService(RegisterSelf = true)]
public sealed class UserService : IUserService;
```

この場合は `IUserService` に加えて `UserService` 自身も登録されます。

## ライフタイム

既定のライフタイムは `Scoped` です。`Lifetime` プロパティで変更できます。

```csharp
using Microsoft.Extensions.DependencyInjection;

[AutoRegisterService(Lifetime = ServiceLifetime.Singleton)]
public sealed class ApplicationClock;

[AutoRegisterService(Lifetime = ServiceLifetime.Transient)]
public sealed class MessageFormatter;
```

`Scoped`、`Singleton`、`Transient` を指定できます。

## 登録結果を受け取る

第 2 引数のコールバックは、サービスが 1 件登録されるたびに呼び出されます。

```csharp
builder.Services.AddAutoRegisterServices<UserService>(registered =>
{
    Console.WriteLine(
        $"{registered.ServiceType.Name} -> "
        + $"{registered.ImplementationType.Name} ({registered.Lifetime})"
    );
});
```

コールバックには、サービス型、実装型、ライフタイムを持つ
`ServiceTypePair` が渡されます。

コールバックは各サービスを DI コンテナーへ追加した後に呼び出されます。コールバックから
例外を送出しないでください。例外が送出された場合、それ以前に追加されたサービスは
コレクションに残ります。

## 注意事項

- 走査対象は、`AddAutoRegisterServices` に渡した型が属するアセンブリだけです。
- 抽象クラスは登録されません。
- 未確定の型パラメーターを含むオープンジェネリック実装型はサポートしていません。
  そのような型へ `[AutoRegisterService]` を付けると、登録時に
  `InvalidOperationException` が発生し、サービスは登録されません。
- `IRepository<User>` のように型引数が確定したインターフェイスを実装する
  非ジェネリック具象クラスは登録できます。
- 同じサービス型が既に登録されていても置き換えは行わず、通常の
  `AddScoped` などと同様に登録を追加します。
- `AutoRegisterServiceAttribute` は派生クラスへ継承されません。登録対象にする具象クラス
  ごとに属性を直接付けてください。基底クラスにだけ属性が付いている場合、その派生クラス
  は登録されません。
