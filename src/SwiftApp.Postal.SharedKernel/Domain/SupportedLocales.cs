namespace SwiftApp.Postal.SharedKernel.Domain;

/// <summary>
/// Supported locales for the Swiss Postal platform.
/// Default is German (de), with French (fr), Italian (it), and English (en).
/// </summary>
public static class SupportedLocales
{
    public const string De = "de";
    public const string Fr = "fr";
    public const string It = "it";
    public const string En = "en";
    public const string Default = De;

    public static readonly IReadOnlyList<string> All = [De, Fr, It, En];

    public static bool IsSupported(string? locale) =>
        locale is not null && All.Contains(locale.ToLowerInvariant());

    public static string Normalize(string? locale)
    {
        if (locale is null) return Default;
        var lower = locale.ToLowerInvariant();
        if (lower.Contains('-'))
            lower = lower.Split('-')[0];
        return IsSupported(lower) ? lower : Default;
    }
}
