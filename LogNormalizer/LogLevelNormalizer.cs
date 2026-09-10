namespace LogNormalizer;


/// Приводит уровень логирования из входной записи к одному из выходных значений:
/// INFO, WARN, ERROR, DEBUG.

public static class LogLevelNormalizer
{
    private static readonly Dictionary<string, string> Levels = new(StringComparer.OrdinalIgnoreCase)
    {
        ["INFORMATION"] = "INFO",
        ["INFO"] = "INFO",
        ["WARNING"] = "WARN",
        ["WARN"] = "WARN",
        ["ERROR"] = "ERROR",
        ["DEBUG"] = "DEBUG",
    };

    public static bool TryNormalize(string rawLevel, out string normalizedLevel) =>
        Levels.TryGetValue(rawLevel, out normalizedLevel!);
}
