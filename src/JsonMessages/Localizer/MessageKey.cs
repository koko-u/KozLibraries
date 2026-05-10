using System.Text.Json.Serialization;
using KozLibraries.JsonMessages.Converters;

namespace KozLibraries.JsonMessages.Localizer;

[JsonConverter(typeof(MessageKeyConverter))]
public readonly record struct MessageKey(string Key)
{
    public static implicit operator string(MessageKey key) => key.Key;

    public static implicit operator MessageKey(string key) => new(key);

    public override string ToString() => Key;
}
