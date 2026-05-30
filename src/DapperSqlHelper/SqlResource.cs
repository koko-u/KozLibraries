using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace KozLibraries.DapperSqlHelper;

/// <summary>
/// プロジェクトルートの所定のフォルダ配下の SQL リソースファイルを管理するクラスです
/// </summary>
[PublicAPI]
public sealed class SqlResource
{
    private readonly IFileProvider _fileProvider;
    private readonly SqlResourceOption _option;

    /// <summary>
    /// プロジェクトルートの所定のフォルダ配下の SQL リソースファイルを管理するクラスです
    /// </summary>
    public SqlResource(IHostEnvironment env, IOptions<SqlResourceOption> options)
    {
        _fileProvider = options.Value.Assembly is null
            ? env.ContentRootFileProvider
            : new ManifestEmbeddedFileProvider(options.Value.Assembly);
        _option = options.Value;

        if (options.Value.Assembly is not null)
        {
            var hasManifest = options
                .Value.Assembly.GetManifestResourceNames()
                .Any(x =>
                    x.EndsWith(
                        "Microsoft.Extensions.FileProviders.Embedded.Manifest.xml",
                        StringComparison.Ordinal
                    )
                );
            if (!hasManifest)
            {
                throw new InvalidOperationException(
                    """
                    Embedded manifest file not found in the assembly.
                    Your project should contain 'GenerateEmbeddedFilesManifest' property set to true.
                    """
                );
            }
        }
    }

    /// <summary>
    /// Get Sql query from file
    /// </summary>
    /// <param name="sqlFilePath">SQL file path</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    public async Task<string> GetAsync(string sqlFilePath, CancellationToken cancellationToken)
    {
        var sqlFile = _fileProvider.GetFileInfo($"{_option.SqlBasePath}/{sqlFilePath}");
        if (!sqlFile.Exists)
        {
            throw new FileNotFoundException(
                $"SQL file not found {_option.SqlBasePath}/{sqlFilePath}"
            );
        }

        await using var stream = sqlFile.CreateReadStream();
        using var reader = new StreamReader(stream);

        return await reader.ReadToEndAsync(cancellationToken);
    }
}
