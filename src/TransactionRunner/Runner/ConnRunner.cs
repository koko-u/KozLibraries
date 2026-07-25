using System;
using System.Threading;
using System.Threading.Tasks;
using KozLibraries.TransactionRunner.Data;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace KozLibraries.TransactionRunner.Runner;

/// <summary>
/// Non-Transactional Runner
/// </summary>
public sealed class ConnRunner(NpgsqlDataSource dataSource, ILogger<ConnRunner> logger)
{
    /// <summary>
    /// Execute action in the Non-Transaction
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T">return value type of the action</typeparam>
    /// <returns></returns>
    public async Task<T> ExecuteAsync<T>(
        Func<ConnSession, CancellationToken, Task<T>> action,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await dataSource.OpenConnectionAsync(cancellationToken);

        try
        {
            return await action(new ConnSession(conn), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected Error has occurred.");
            throw;
        }
    }

    /// <summary>
    /// Execute action in a Non-Transaction
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationToken"></param>
    public async Task ExecuteAsync(
        Func<ConnSession, CancellationToken, Task> action,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await dataSource.OpenConnectionAsync(cancellationToken);

        try
        {
            await action(new ConnSession(conn), cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected Error has occurred.");
            throw;
        }
    }
}
