namespace KozLibraries.JsonMessages.Localizer;

public readonly record struct LocaleKey(string CultureName)
{
    public static implicit operator string(LocaleKey key) => key.CultureName;

    public static implicit operator LocaleKey(string cultureName) => new(cultureName);

    public override string ToString() => CultureName;
}
