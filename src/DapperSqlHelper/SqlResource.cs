using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace KozLibraries.DapperSqlHelper;

/// <summary>
/// プロジェクトルートの所定のフォルダ配下の SQL リソースファイルを管理するクラスです
/// </summary>
[PublicAPI]
public sealed class SqlResource(IHostEnvironment env, IConfiguration config)
{
    private readonly string _sqlRoot = config["SqlHelper:Root"] ?? "Sql";
    private readonly IFileProvider _fileProvider = env.ContentRootFileProvider;

    public Task<string> GetAsync(string sqlFilePath, CancellationToken cancellationToken)
    {
        var sqlFile = _fileProvider.GetFileInfo($"{_sqlRoot}/{sqlFilePath}");
        var sqlPath =
            sqlFile.PhysicalPath
            ?? throw new FileNotFoundException(
                $"SQL file not found: {sqlFilePath} under {_sqlRoot}"
            );

        return File.ReadAllTextAsync(sqlPath, cancellationToken);
    }
}
