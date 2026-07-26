# KozLibraries

KozLibraries は、小さく再利用しやすい .NET ライブラリをまとめたリポジトリです。

## プロジェクト

| プロジェクト | 概要 | 詳細 |
| --- | --- | --- |
| KozLibraries.AutoRegisterAnnotation | 属性を付けたクラスを、指定したアセンブリから `Microsoft.Extensions.DependencyInjection` の DI コンテナーへ登録します。 | [README](src/AutoRegisterAnnotation/README.md) |
| KozLibraries.DapperDateOnlySupport | Dapper で .NET の `DateOnly` を扱うための型ハンドラーを提供します。 | [README](src/DapperDateOnlySupport/README.md) |
| KozLibraries.DapperSqlHelper | コンテンツルートまたはアセンブリに埋め込まれた `.sql` ファイルを非同期に読み込みます。 | [README](src/DapperSqlHelper/README.md) |
| KozLibraries.JsonMessages | ASP.NET Core アプリケーションで、現在の UI カルチャに応じたメッセージを JSON または JSONC ファイルから読み込みます。 | [README](src/JsonMessages/README.md) |
| KozLibraries.TagHelpers | Razor Pages のナビゲーションや Bootstrap の検証表示を補助する Tag Helper 集です。 | [README](src/TagHelpers/README.md) |
| KozLibraries.TransactionRunner | Npgsql の接続管理と、トランザクションの開始・コミット・ロールバックを定型化します。 | [README](src/TransactionRunner/README.md) |
