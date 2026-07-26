# KozLibraries.TagHelpers

`KozLibraries.TagHelpers` は、ASP.NET Core Razor Pages でよく使う HTML 出力を補助する
Tag Helper 集です。

次の機能を提供します。

- 現在の Razor Page に対応するナビゲーションリンクへ `active` クラスを追加
- 現在の Razor Page のパスが指定した接頭辞で始まるリンクへ `active` クラスを追加
- ASP.NET Core の検証結果に応じて Bootstrap の検証用クラスを追加
- 検証メッセージへ Bootstrap の `invalid-feedback` クラスを追加

## インストール

```sh
dotnet add package KozLibraries.TagHelpers
```

## Tag Helper を有効にする

Razor Pages プロジェクトの `Pages/_ViewImports.cshtml` に追加します。

```razor
@addTagHelper *, KozLibraries.TagHelpers
```

既存の ASP.NET Core Tag Helper も使用する場合は、通常どおり次の指定も残します。

```razor
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, KozLibraries.TagHelpers
```

## 現在のページと一致するリンクを強調する

`asp-page` を持つ `<a>` 要素へ `active-when-page` を付けます。

```razor
<a asp-page="/Index" active-when-page class="nav-link">ホーム</a>
<a asp-page="/Users/Index" active-when-page class="nav-link">ユーザー</a>
```

現在のページが `/Users/Index` の場合、2 番目のリンクには次のように `active` クラスが
追加されます。

```html
<a class="nav-link active" href="/Users">ユーザー</a>
```

ページ名の比較では大文字と小文字を区別しません。`active-when-page` 属性は最終的な
HTML から削除されます。

## ページパスの接頭辞でリンクを強調する

複数のページを同じナビゲーション項目として扱いたい場合は、
`active-when-prefix` にページパスの接頭辞を指定します。

```razor
<a asp-page="/Users/Index"
   active-when-prefix="/Users"
   class="nav-link">
    ユーザー管理
</a>
```

現在のページが `/Users/Index` や `/Users/Edit` の場合に `active` クラスが追加されます。
接頭辞の比較では大文字と小文字を区別しません。有効な接頭辞を指定した場合、
`active-when-prefix` 属性は最終的な HTML から削除されます。

## Bootstrap の検証表示を追加する

入力要素へ `bs-valid`、検証メッセージ要素へ `bs-feedback` を追加します。

```razor
<form method="post">
    <div class="mb-3">
        <label asp-for="Input.Email" class="form-label"></label>
        <input asp-for="Input.Email" bs-valid class="form-control" />
        <span asp-validation-for="Input.Email" bs-feedback></span>
    </div>

    <button type="submit" class="btn btn-primary">送信</button>
</form>
```

`bs-valid` は、対象フィールドが `ModelState` に存在する場合に検証結果を確認します。

- エラーがある場合は `is-invalid` クラスを追加し、`aria-describedby` を
  `{フィールド名}_Feedback` に設定します。
- エラーがない場合は `is-valid` クラスを追加します。
- `type="hidden"` の `<input>` は処理しません。

`bs-feedback` は検証メッセージ要素へ `invalid-feedback` クラスを追加し、`id` を
`{フィールド名}_Feedback` に設定します。この ID は `bs-valid` が設定する
`aria-describedby` と対応します。

ネストしたモデルではフィールド名に `.` が含まれるため、たとえば `Input.Email` の
検証メッセージ ID は `Input.Email_Feedback` になります。

## 注意事項

- このライブラリは Bootstrap 本体の CSS や JavaScript を同梱しません。Bootstrap は
  アプリケーション側で導入してください。
- `bs-valid` は `ModelState` に対象フィールドがまだ存在しない初回表示では、検証クラスを
  追加しません。
- 既存の `class` 属性は保持され、その末尾へ必要なクラスが追加されます。
