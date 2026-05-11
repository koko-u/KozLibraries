using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using JetBrains.Annotations;
using KozLibraries.JsonMessages.Localizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace KozLibraries.JsonMessages;

public sealed class JsonMessageLocalizer(
    IWebHostEnvironment environment,
    IConfiguration configuration
)
{
    private LocaleKey DefaultCulture => configuration["DefaultCulture"] ?? "en";
    private readonly ConcurrentDictionary<LocaleKey, Dictionary<MessageKey, string>> _messageCache =
        new();

    /// <summary>
    /// Get Localized message by key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    [PublicAPI]
    public string Get(MessageKey key)
    {
        var culture = CultureInfo.CurrentUICulture;

        if (this.TryGet(culture.Name, key, out var message1))
        {
            return message1;
        }

        if (this.TryGet(culture.TwoLetterISOLanguageName, key, out var message2))
        {
            return message2;
        }

        if (this.TryGet(DefaultCulture, key, out var message3))
        {
            return message3;
        }

        return key.Key;
    }

    /// <summary>
    /// Format localized message by key with params
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    [PublicAPI]
    public string Format(MessageKey key, object values)
    {
        var template = this.Get(key);

        foreach (var propertyInfo in values.GetType().GetProperties())
        {
            var placeholder = $"{{{propertyInfo.Name}}}";
            var value = propertyInfo.GetValue(values)?.ToString() ?? string.Empty;

            template = template.Replace(placeholder, value);
        }

        return template;
    }

    /// <summary>
    /// Try get localized message by culture and key
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    [PublicAPI]
    public bool TryGet(LocaleKey locale, MessageKey key, out string value)
    {
        var messages = _messageCache.GetOrAdd(locale, ReadFromResourceFile);
        if (messages.TryGetValue(key, out var s))
        {
            value = s;
            return true;
        }
        value = null!;
        return false;
    }

    /// <summary>
    /// Read messages file, then convert into dictionary
    /// </summary>
    /// <param name="locale"></param>
    /// <returns></returns>
    private Dictionary<MessageKey, string> ReadFromResourceFile(LocaleKey locale)
    {
        var jsonPath = JsonPath(locale);
        if (jsonPath is null)
        {
            return [];
        }

        using var stream = File.OpenRead(jsonPath);
        var messages = JsonSerializer.Deserialize<Dictionary<MessageKey, string>>(
            stream,
            new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true,
            }
        );

        return messages ?? [];
    }

    /// <summary>
    /// Get messages json path for locale
    /// </summary>
    /// <param name="locale"></param>
    /// <returns>
    /// Resources/messages.{locale}.json path or Resources/messages.{locale}.jsonc path
    /// otherwise return null.
    /// </returns>
    private string? JsonPath(LocaleKey locale)
    {
        // json extension file
        var path1 = Path.Combine(
            environment.ContentRootPath,
            "Resources",
            $"messages.{locale}.json"
        );
        if (File.Exists(path1))
        {
            return path1;
        }

        // jsonc extension file
        var path2 = Path.Combine(
            environment.ContentRootPath,
            "Resources",
            $"messages.{locale}.jsonc"
        );
        if (File.Exists(path2))
        {
            return path2;
        }

        return null;
    }
}
