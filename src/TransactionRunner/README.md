# KozLibraries.TransactionRunner

`KozLibraries.TransactionRunner` は、Npgsql の接続とトランザクションの開始・終了を
定型化するためのライブラリです。

処理をデリゲートとして渡すと、接続のオープンと破棄を自動で行います。
トランザクション版では、正常終了時のコミットと例外発生時のロールバックも行います。

## インストール

```sh
dotnet add package KozLibraries.TransactionRunner
```

## DI コンテナーへ登録する

このライブラリは `NpgsqlDataSource` を使用します。最初に Npgsql のデータソースと
TransactionRunner を登録します。

```csharp
using KozLibraries.TransactionRunner;

builder.Services.AddNpgsqlDataSource(
    builder.Configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string is not configured.")
);
builder.Services.AddTransactionRunner();
```

`AddTransactionRunner` は `TxRunner` と `ConnRunner` を Scoped サービスとして
登録します。

## トランザクション内で処理する

`TxRunner.ExecuteAsync` に処理を渡します。デリゲートには、同じ
`NpgsqlConnection` と `NpgsqlTransaction` を持つ `TxSession` が渡されます。

```csharp
using KozLibraries.TransactionRunner.Runner;
using Npgsql;

public sealed class OrderService(TxRunner txRunner)
{
    public Task CreateAsync(
        int customerId,
        CancellationToken cancellationToken
    )
    {
        return txRunner.ExecuteAsync(
            async (session, ct) =>
            {
                await using var command = new NpgsqlCommand(
                    """
                    insert into orders (customer_id)
                    values (@customerId)
                    """,
                    session.Connection,
                    session.Transaction
                );
                command.Parameters.AddWithValue("customerId", customerId);

                await command.ExecuteNonQueryAsync(ct);
            },
            cancellationToken
        );
    }
}
```

デリゲートが正常終了するとコミットされます。デリゲートが例外を送出すると、エラーを
ログへ記録してロールバックした後、同じ例外を呼び出し元へ再送出します。

戻り値が必要な処理には、ジェネリック版を利用できます。

```csharp
var orderId = await txRunner.ExecuteAsync(
    async (session, ct) =>
    {
        await using var command = new NpgsqlCommand(
            """
            insert into orders (customer_id)
            values (@customerId)
            returning id
            """,
            session.Connection,
            session.Transaction
        );
        command.Parameters.AddWithValue("customerId", customerId);

        return (int)(await command.ExecuteScalarAsync(ct))!;
    },
    cancellationToken
);
```

## トランザクションなしで接続を利用する

トランザクションが不要な読み取りなどには `ConnRunner` を使用します。

```csharp
using KozLibraries.TransactionRunner.Runner;
using Npgsql;

public sealed class CustomerQuery(ConnRunner connRunner)
{
    public Task<string?> GetNameAsync(
        int id,
        CancellationToken cancellationToken
    )
    {
        return connRunner.ExecuteAsync(
            async (session, ct) =>
            {
                await using var command = new NpgsqlCommand(
                    "select name from customers where id = @id",
                    session.Connection
                );
                command.Parameters.AddWithValue("id", id);

                return (string?)await command.ExecuteScalarAsync(ct);
            },
            cancellationToken
        );
    }
}
```

`ConnRunner` も、デリゲートの終了後に接続を破棄します。例外はログへ記録して
呼び出し元へ再送出します。

## Dapper と組み合わせる

セッションから接続とトランザクションを取り出し、Dapper の
`CommandDefinition` へ渡せます。

```csharp
await txRunner.ExecuteAsync(
    async (session, ct) =>
    {
        await session.Connection.ExecuteAsync(
            new CommandDefinition(
                "update accounts set balance = balance - @amount where id = @id",
                new { id, amount },
                transaction: session.Transaction,
                cancellationToken: ct
            )
        );
    },
    cancellationToken
);
```

Dapper はこのパッケージの依存関係には含まれないため、組み合わせる場合は
アプリケーション側で別途追加してください。

## 注意事項

- `TxRunner` が管理するトランザクションを確実に利用するため、コマンドや Dapper の
  呼び出しには `session.Transaction` を渡してください。
- 接続やトランザクションをデリゲートの外へ保持しないでください。デリゲートの終了後に
  破棄されます。
- `CancellationToken` は接続、トランザクション、コミット、ロールバックにも渡されます。
