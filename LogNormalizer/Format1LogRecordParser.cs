using System.Globalization;
using System.Text.RegularExpressions;

namespace LogNormalizer;

/// Формат 1: "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'".
/// Поля: Дата Время Уровень Сообщение. Поле "ВызвавшийМетод" отсутствует в этом формате.
public sealed partial class Format1LogRecordParser : ILogRecordParser
{
    [GeneratedRegex(@"^(?<date>\d{2}\.\d{2}\.\d{4}) (?<time>\d{2}:\d{2}:\d{2}\.\d+) (?<level>[A-Za-z]+) (?<message>.+)$")]
    private static partial Regex Pattern();

    public bool TryParse(string line, out LogRecord? record)
    {
        record = null;

        var match = Pattern().Match(line);
        if (!match.Success)
            return false;

        if (!DateOnly.TryParseExact(match.Groups["date"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return false;

        if (!LogLevelNormalizer.TryNormalize(match.Groups["level"].Value, out var level))
            return false;

        record = new LogRecord(date, match.Groups["time"].Value, level, LogRecord.DefaultCallerMethod, match.Groups["message"].Value);
        return true;
    }
}
