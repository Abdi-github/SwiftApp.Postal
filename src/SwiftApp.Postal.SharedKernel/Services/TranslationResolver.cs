using SwiftApp.Postal.SharedKernel.Domain;

namespace SwiftApp.Postal.SharedKernel.Services;

/// <summary>
/// Resolves translated values from translation collections with locale fallback.
/// Fallback order: requested locale → de → first available → default value.
/// </summary>
public static class TranslationResolver
{
    public static string Resolve<T>(
        IEnumerable<T>? translations,
        string locale,
        Func<T, string?> valueSelector,
        string defaultValue = "") where T : BaseTranslation
    {
        if (translations is null) return defaultValue;
        var list = translations.ToList();
        if (list.Count == 0) return defaultValue;

        var normalized = SupportedLocales.Normalize(locale);

        // Try exact locale match
        var match = list.FirstOrDefault(t => t.Locale == normalized);
        if (match is not null)
        {
            var value = valueSelector(match);
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }

        // Fallback to German (default)
        if (normalized != SupportedLocales.De)
        {
            match = list.FirstOrDefault(t => t.Locale == SupportedLocales.De);
            if (match is not null)
            {
                var value = valueSelector(match);
                if (!string.IsNullOrWhiteSpace(value)) return value;
            }
        }

        // Fallback to any available
        foreach (var t in list)
        {
            var value = valueSelector(t);
            if (!string.IsNullOrWhiteSpace(value)) return value;
        }

        return defaultValue;
    }
}
