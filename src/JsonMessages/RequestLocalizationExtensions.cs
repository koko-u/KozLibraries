using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace KozLibraries.JsonMessages;

/// <summary>
/// Extensions for configuring request localization with JSON message support.
/// </summary>
[PublicAPI]
public static class RequestLocalizationExtensions
{
    /// <summary>
    /// add support for JSON message localization
    /// </summary>
    /// <param name="services"></param>
    /// <param name="supportedCultures"></param>
    /// <returns></returns>
    public static IServiceCollection ConfigureRequestLocalization(
        this IServiceCollection services,
        Func<IEnumerable<CultureInfo>> supportedCultures
    )
    {
        var cultures = supportedCultures().ToList();
        var defaultCulture = cultures.FirstOrDefault() ?? new CultureInfo("en");

        services.AddSingleton<JsonMessageLocalizer>();
        services.Configure<RequestLocalizationOptions>(opts =>
        {
            opts.DefaultRequestCulture = new RequestCulture(defaultCulture);
            opts.SupportedCultures = cultures;
            opts.SupportedUICultures = cultures;

            // use browser's Accepts header
            opts.RequestCultureProviders = [new AcceptLanguageHeaderRequestCultureProvider()];
        });

        return services;
    }
}
