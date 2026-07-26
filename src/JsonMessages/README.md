# KozLibraries.JsonMessages

`KozLibraries.JsonMessages` は、ASP.NET Core アプリケーションで JSON ファイルから
メッセージを読み込み、現在の UI カルチャに応じた文字列を取得するための
シンプルなローカライズライブラリです。

`.json` と、コメントや末尾のカンマを記述できる `.jsonc` に対応しています。

## インストール

```sh
dotnet add package KozLibraries.JsonMessages
```

## メッセージファイルを用意する

アプリケーションのコンテンツルート直下に `Resources` ディレクトリを作り、
`messages.{カルチャ名}.json` または `messages.{カルチャ名}.jsonc` という名前で
ファイルを配置します。

```text
MyApplication/
├── Resources/
│   ├── messages.en.json
│   ├── messages.ja.json
│   └── messages.ja-JP.jsonc
└── MyApplication.csproj
```

ファイルには、メッセージキーと文字列の組み合わせを JSON オブジェクトとして
記述します。

```json
{
  "Welcome": "ようこそ",
  "Greeting": "こんにちは、{Name}さん",
  "ItemsFound": "{Count} 件見つかりました"
}
```

## ASP.NET Core へ登録する

`Program.cs` で、対応するカルチャを指定します。

```csharp
using System.Globalization;
using KozLibraries.JsonMessages;

builder.Services.ConfigureRequestLocalization(() =>
[
    new CultureInfo("ja-JP"),
    new CultureInfo("en")
]);

var app = builder.Build();

app.UseRequestLocalization();
```

一覧の先頭のカルチャが、リクエストローカライゼーションの既定カルチャになります。
カルチャの判定にはブラウザーの `Accept-Language` ヘッダーが使われます。

## メッセージを取得する

`JsonMessageLocalizer` を DI で受け取り、`Get` を呼び出します。
メッセージキーには文字列からの暗黙変換が用意されています。

```csharp
using KozLibraries.JsonMessages;

public sealed class WelcomeService(JsonMessageLocalizer localizer)
{
    public string GetTitle()
    {
        return localizer.Get("Welcome");
    }
}
```

キーを型として明示することもできます。

```csharp
using KozLibraries.JsonMessages.Localizer;

MessageKey key = new("Welcome");
var message = localizer.Get(key);
```

## プレースホルダーを置換する

`Format` は、匿名オブジェクトなどの public プロパティ名と一致する
`{プロパティ名}` を置換します。

```csharp
var greeting = localizer.Format("Greeting", new { Name = "佐藤" });
var result = localizer.Format("ItemsFound", new { Count = 3 });
```

置換はプロパティ値の `ToString()` を使った単純な文字列置換です。
複合書式指定やカルチャを指定した数値・日付の書式設定は行いません。

## メッセージ検索の順序

`Get` は、次の順番でメッセージを探します。

1. 現在の UI カルチャの完全な名前（例: `ja-JP`）
2. 現在の UI カルチャの 2 文字 ISO 言語名（例: `ja`）
3. 構成値 `DefaultCulture` のカルチャ（未設定の場合は `en`）
4. どのファイルにもキーがなければ、メッセージキー自身

既定のフォールバックカルチャを変更する場合は、`appsettings.json` などに設定します。

```json
{
  "DefaultCulture": "ja"
}
```

特定のカルチャを指定して検索したい場合は `TryGet` を利用できます。

```csharp
if (localizer.TryGet("en", "Welcome", out var message))
{
    Console.WriteLine(message);
}
```

## ファイル形式とキャッシュ

- 同じカルチャでは、最初の読み込み結果がメモリにキャッシュされます。
- アプリケーション実行中にファイルを変更しても、自動的には再読み込みされません。
- 同じカルチャについて `.json` と `.jsonc` の両方がある場合は `.json` を優先します。
- `.jsonc` ではコメントと末尾のカンマを使用できます。
- ファイルが存在しない場合は空のメッセージ集合として扱われます。
