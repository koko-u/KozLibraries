using System.Reflection;

namespace KozLibraries.DapperSqlHelper;

/// <summary>
/// Option for SqlResource service
/// </summary>
public sealed class SqlResourceOption
{
    /// <summary>
    /// base path for sql resource files
    /// </summary>
    public string SqlBasePath { get; set; } = "Sql";

    /// <summary>
    /// Assembly to load sql resource files from
    /// </summary>
    public Assembly? Assembly { get; set; }
}
