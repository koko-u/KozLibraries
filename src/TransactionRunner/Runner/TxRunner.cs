using System;
using System.Threading;
using System.Threading.Tasks;
using KozLibraries.TransactionRunner.Data;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace KozLibraries.TransactionRunner.Runner;

/// <summary>
/// Transaction Scoped Runner
/// </summary>
/// <param name="dataSource"></param>
/// <param name="logger"></param>
public sealed class TxRunner(NpgsqlDataSource dataSource, ILogger<TxRunner> logger)
{
    /// <summary>
    /// Execute action in the Db Transaction
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">return value type of the action</typeparam>
    /// <returns></returns>
    public async Task<T> ExecuteAsync<T>(
        Func<TxSession, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var tx = await conn.BeginTransactionAsync(cancellationToken);

        try
        {
            var result = await action(new TxSession(conn, tx), cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected Error has occurred.");
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    /// <summary>
    /// Execute action in a Db Transaction
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    public async Task ExecuteAsync(Func<TxSession, CancellationToken, Task> action, CancellationToken cancellationToken)
    {
        await using var conn = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var tx = await conn.BeginTransactionAsync(cancellationToken);

        try
        {
            await action(new TxSession(conn, tx), cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected Error has occurred.");
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
