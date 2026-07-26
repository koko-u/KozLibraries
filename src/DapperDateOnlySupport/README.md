# KozLibraries.DapperDateOnlySupport

`KozLibraries.DapperDateOnlySupport` は、Dapper で .NET の `DateOnly` を扱うための
型ハンドラーを提供するライブラリです。

データベースパラメーターを `DbType.Date` として設定し、取得値が `DateOnly`、
`DateTime`、または文字列の場合に `DateOnly` へ変換します。

## インストール

```sh
dotnet add package KozLibraries.DapperDateOnlySupport
```

## セットアップ

アプリケーションの起動時に、Dapper へ `DateOnlyHandler` を登録します。

```csharp
using Dapper;
using KozLibraries.DapperDateOnlySupport;

SqlMapper.AddTypeHandler(new DateOnlyHandler());
```

型ハンドラーの登録はアプリケーション全体に対する設定です。通常は、Dapper を使用する
前に 1 回だけ行います。

## 使用例

次のようなモデルを用意します。

```csharp
public sealed record Person(int Id, string Name, DateOnly Birthday);
```

登録後は、通常の Dapper のクエリで `DateOnly` をパラメーターや結果のプロパティとして
利用できます。

```csharp
var birthday = new DateOnly(2000, 1, 23);

var people = await connection.QueryAsync<Person>(
    """
    select id, name, birthday
    from people
    where birthday >= @birthday
    """,
    new { birthday }
);
```

パラメーターの `birthday` は `DbType.Date` として設定されます。問い合わせ結果は、
データベースドライバーから返された値に応じて次のように変換されます。

- `DateOnly` はそのまま返します。
- `DateTime` は日付部分を `DateOnly` へ変換します。
- 文字列は `DateOnly.Parse` で解析します。
- それ以外の型は `DataException` になります。

## 注意事項

- 時刻やタイムゾーンの情報は保持しません。時刻が必要な列には `DateTime` や
  `DateTimeOffset` を使用してください。
- 文字列からの変換は `DateOnly.Parse` の規則に従います。利用する
  ADO.NET プロバイダーが日付列をどの型で返すかも確認してください。
