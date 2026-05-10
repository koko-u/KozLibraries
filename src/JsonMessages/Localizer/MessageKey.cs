namespace KozLibraries.JsonMessages.Localizer;

public record struct MessageKey(string Key)
{
    public static implicit operator string(MessageKey key) => key.Key;

    public static implicit operator MessageKey(string key) => new(key);

    public override string ToString() => Key;
}
