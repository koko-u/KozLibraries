using System;
using Microsoft.Extensions.DependencyInjection;

namespace KozLibraries.DapperSqlHelper;

public static class SqlResourceExtension
{
    /// <summary>
    /// Register SqlResource service with DI container
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configure">Configuration for SqlResource root Directory (default is "Sql")</param>
    /// <returns></returns>
    public static IServiceCollection AddSqlResource(
        this IServiceCollection services,
        Action<SqlResourceOption> configure
    )
    {
        services.Configure(configure);
        services.AddSingleton<SqlResource>();
        return services;
    }
}
