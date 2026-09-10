using System.Globalization;
using System.Text.RegularExpressions;

namespace LogNormalizer;

/// Формат 2: "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '...'".
/// Поля: ДатаВремя|Уровень|ИдПотока|ВызвавшийМетод|Сообщение, разделены "|".
public sealed partial class Format2LogRecordParser : ILogRecordParser
{
    [GeneratedRegex(@"^(?<date>\d{4}-\d{2}-\d{2}) (?<time>\d{2}:\d{2}:\d{2}\.\d+)\|\s*(?<level>[A-Za-z]+)\|\d+\|(?<caller>[^|]*)\|\s*(?<message>.+)$")]
    private static partial Regex Pattern();

    public bool TryParse(string line, out LogRecord? record)
    {
        record = null;

        var match = Pattern().Match(line);
        if (!match.Success)
            return false;

        if (!DateOnly.TryParseExact(match.Groups["date"].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return false;

        if (!LogLevelNormalizer.TryNormalize(match.Groups["level"].Value, out var level))
            return false;

        string caller = match.Groups["caller"].Value.Trim();
        if (caller.Length == 0)
            caller = LogRecord.DefaultCallerMethod;

        record = new LogRecord(date, match.Groups["time"].Value, level, caller, match.Groups["message"].Value);
        return true;
    }
}
