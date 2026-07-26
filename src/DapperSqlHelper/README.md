# KozLibraries.DapperSqlHelper

`KozLibraries.DapperSqlHelper` は、SQL を C# の文字列内ではなく `.sql` ファイルで管理し、
その内容を非同期に読み込むためのライブラリです。

SQL ファイルは、アプリケーションのコンテンツルートに置く方法と、アセンブリへ
埋め込む方法の 2 種類に対応しています。

## インストール

```sh
dotnet add package KozLibraries.DapperSqlHelper
```

## コンテンツルートから読み込む

まず、SQL ファイルをアプリケーション内へ配置します。

```text
MyApplication/
├── Sql/
│   └── Users/
│       └── FindById.sql
└── MyApplication.csproj
```

`Program.cs` で `SqlResource` を DI コンテナーへ登録します。

```csharp
using KozLibraries.DapperSqlHelper;

builder.Services.AddSqlResource(options =>
{
    options.SqlBasePath = "Sql";
});
```

`SqlBasePath` の既定値は `Sql` なので、既定値を使う場合も設定用のラムダ式は渡します。

SQL を利用するクラスへ `SqlResource` を注入し、ベースパスからの相対パスで読み込みます。

```csharp
using Dapper;
using KozLibraries.DapperSqlHelper;

public sealed class UserRepository(SqlResource sqlResource, DbConnection connection)
{
    public async Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        var sql = await sqlResource.GetAsync(
            "Users/FindById.sql",
            cancellationToken
        );

        return await connection.QuerySingleOrDefaultAsync<User>(
            new CommandDefinition(
                sql,
                new { id },
                cancellationToken: cancellationToken
            )
        );
    }
}
```

`SqlResource` は SQL の読み込みだけを担当します。Dapper での実行やパラメーターの設定は、
呼び出し側で行います。

## SQL ファイルをアセンブリへ埋め込む

ライブラリなど、SQL ファイルを出力ディレクトリへ個別に配置したくない場合は、
埋め込みファイルとして読み込めます。

SQL を含むプロジェクトの `.csproj` を設定します。

```xml
<PropertyGroup>
  <GenerateEmbeddedFilesManifest>true</GenerateEmbeddedFilesManifest>
</PropertyGroup>

<ItemGroup>
  <EmbeddedResource Include="Sql/**/*.sql" />
</ItemGroup>
```

登録時に、その SQL ファイルを埋め込んだアセンブリを指定します。

```csharp
builder.Services.AddSqlResource(options =>
{
    options.SqlBasePath = "Sql";
    options.Assembly = typeof(UserRepository).Assembly;
});
```

`Assembly` が設定されている場合は `ManifestEmbeddedFileProvider` が使われます。
埋め込みファイルマニフェストが生成されていない場合は、`SqlResource` の生成時に
`InvalidOperationException` が発生します。

## エラーと注意事項

- 指定した SQL ファイルが存在しない場合は `FileNotFoundException` が発生します。
- `GetAsync` に渡すパスは `SqlBasePath` からの相対パスです。
- `SqlResource` は DI コンテナーへ Singleton として登録されます。
- SQL の内容はキャッシュされません。`GetAsync` を呼び出すたびにファイルを開いて
  読み込みます。
