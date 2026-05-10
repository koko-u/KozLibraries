using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KozLibraries.JsonMessages.Localizer;

namespace KozLibraries.JsonMessages.Converters;

public sealed class LocaleKeyConverter : JsonConverter<LocaleKey>
{
    public override LocaleKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token for LocaleKey");
        }

        var cultureName =
            reader.GetString() ?? throw new JsonException("null-value is not allowed.");

        return new LocaleKey(cultureName);
    }

    public override void Write(
        Utf8JsonWriter writer,
        LocaleKey value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.CultureName);
    }
}
