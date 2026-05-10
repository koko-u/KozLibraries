using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using KozLibraries.JsonMessages.Localizer;

namespace KozLibraries.JsonMessages.Converters;

public sealed class MessageKeyConverter : JsonConverter<MessageKey>
{
    public override MessageKey Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected string token for MessageKey");
        }

        var key = reader.GetString() ?? throw new JsonException("null-value is not allowed.");

        return new MessageKey(key);
    }

    public override void Write(
        Utf8JsonWriter writer,
        MessageKey value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Key);
    }

    public override MessageKey ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var key = reader.GetString() ?? throw new JsonException("null-value is not allowed.");

        return new MessageKey(key);
    }

    public override void WriteAsPropertyName(
        Utf8JsonWriter writer,
        MessageKey value,
        JsonSerializerOptions options
    )
    {
        writer.WriteStringValue(value.Key);
    }
}
