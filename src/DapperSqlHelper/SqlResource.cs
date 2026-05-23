using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KozLibraries.DapperSqlHelper;

/// <summary>
/// プロジェクトルートの所定のフォルダ配下の SQL リソースファイルを管理するクラスです
/// </summary>
[PublicAPI]
public sealed class SqlResource(IHostEnvironment env, IOptions<SqlResourceOption> options)
{
    private readonly IFileProvider _fileProvider = env.ContentRootFileProvider;
    private readonly SqlResourceOption _option = options.Value;

    /// <summary>
    /// Get Sql query from file
    /// </summary>
    /// <param name="sqlFilePath">SQL file path</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public Task<string> GetAsync(string sqlFilePath, CancellationToken cancellationToken)
    {
        var sqlFile = _fileProvider.GetFileInfo($"{_option.SqlBasePath}/{sqlFilePath}");
        var sqlPath =
            sqlFile.PhysicalPath
            ?? throw new FileNotFoundException(
                $"SQL file not found: {sqlFilePath} under {_option.SqlBasePath}"
            );

        return File.ReadAllTextAsync(sqlPath, cancellationToken);
    }
}
