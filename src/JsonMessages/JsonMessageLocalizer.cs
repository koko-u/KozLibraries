using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using KozLibraries.JsonMessages.Localizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace KozLibraries.JsonMessages;

public sealed class JsonMessageLocalizer(
    IWebHostEnvironment environment,
    IConfiguration configuration
)
{
    private LocaleKey DefaultCulture => new LocaleKey(configuration["DefaultCulture"] ?? "en");
    private readonly ConcurrentDictionary<LocaleKey, Dictionary<MessageKey, string>> _messageCache =
        new();

    /// <summary>
    /// Get Localized message by key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public string Get(MessageKey key)
    {
        var culture = CultureInfo.CurrentUICulture;

        if (this.TryGet(new LocaleKey(culture.Name), key, out var message1))
        {
            return message1;
        }

        if (this.TryGet(new LocaleKey(culture.TwoLetterISOLanguageName), key, out var message2))
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
    /// Format localized messsage by key with params
    /// </summary>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <returns></returns>
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
    /// Try get localized messsage by culture and key
    /// </summary>
    /// <param name="locale"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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
        var path = Path.Combine(
            environment.ContentRootPath,
            "Resources",
            $"messages.{locale}.json"
        );
        if (!File.Exists(path))
        {
            return [];
        }

        using var stream = File.OpenRead(path);
        var messages = JsonSerializer.Deserialize<Dictionary<MessageKey, string>>(stream);

        return messages ?? [];
    }
}
