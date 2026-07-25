using KozLibraries.TransactionRunner.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace KozLibraries.TransactionRunner;

public static class TransactionRunnerServiceExtension
{
    public static IServiceCollection AddTransactionRunner(this IServiceCollection services)
    {
        services.AddScoped<TxRunner>();
        services.AddScoped<ConnRunner>();

        return services;
    }
}
