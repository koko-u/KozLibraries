using System.Text.Json.Serialization;
using KozLibraries.JsonMessages.Converters;

namespace KozLibraries.JsonMessages.Localizer;

[JsonConverter(typeof(LocaleKeyConverter))]
public readonly record struct LocaleKey(string CultureName)
{
    public static implicit operator string(LocaleKey key) => key.CultureName;

    public static implicit operator LocaleKey(string cultureName) => new(cultureName);

    public override string ToString() => CultureName;
}
