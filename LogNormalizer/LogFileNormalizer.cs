namespace LogNormalizer;


/// Приводит записи лог-файла к единому формату. Строки, не подошедшие ни под один
/// из зарегистрированных парсеров, считаются невалидными и переносятся как есть.

public sealed class LogFileNormalizer
{
    private readonly IReadOnlyList<ILogRecordParser> _parsers;

    public LogFileNormalizer(IReadOnlyList<ILogRecordParser> parsers)
    {
        if (parsers.Count == 0)
            throw new ArgumentException("Нужен хотя бы один парсер.", nameof(parsers));

        _parsers = parsers;
    }

    public NormalizationResult Normalize(TextReader input, TextWriter output, TextWriter problems)
    {
        int validCount = 0;
        int invalidCount = 0;

        string? line;
        while ((line = input.ReadLine()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (TryParseWithAnyParser(line, out var record))
            {
                output.WriteLine(record!.ToOutputLine());
                validCount++;
            }
            else
            {
                problems.WriteLine(line);
                invalidCount++;
            }
        }

        return new NormalizationResult(validCount, invalidCount);
    }

    private bool TryParseWithAnyParser(string line, out LogRecord? record)
    {
        foreach (var parser in _parsers)
        {
            if (parser.TryParse(line, out record))
                return true;
        }

        record = null;
        return false;
    }
}

public readonly record struct NormalizationResult(int ValidCount, int InvalidCount);
