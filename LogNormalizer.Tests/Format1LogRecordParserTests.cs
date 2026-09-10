using LogNormalizer;
using Xunit;

namespace LogNormalizer.Tests;

public class Format1LogRecordParserTests
{
    private readonly Format1LogRecordParser _parser = new();

    [Fact]
    public void TryParse_ParsesValidLine()
    {
        string line = "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'";

        bool ok = _parser.TryParse(line, out var record);

        Assert.True(ok);
        Assert.Equal(new DateOnly(2025, 3, 10), record!.Date);
        Assert.Equal("15:14:49.523", record.Time);
        Assert.Equal("INFO", record.Level);
        Assert.Equal(LogRecord.DefaultCallerMethod, record.CallerMethod);
        Assert.Equal("Версия программы: '3.4.0.48729'", record.Message);
    }

    [Fact]
    public void TryParse_NormalizesWarningLevel()
    {
        string line = "10.03.2025 15:14:49.523 WARNING Диск почти заполнен";

        bool ok = _parser.TryParse(line, out var record);

        Assert.True(ok);
        Assert.Equal("WARN", record!.Level);
    }

    [Theory]
    [InlineData("")]
    [InlineData("это не лог-строка вообще")]
    [InlineData("2025-03-10 15:14:49.523 INFORMATION Не тот формат даты")]
    [InlineData("10.03.2025 15:14:49.523 TRACE Неизвестный уровень логирования")]
    public void TryParse_RejectsInvalidLines(string line)
    {
        bool ok = _parser.TryParse(line, out var record);

        Assert.False(ok);
        Assert.Null(record);
    }
}
