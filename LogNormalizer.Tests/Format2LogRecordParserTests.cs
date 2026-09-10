using LogNormalizer;
using Xunit;

namespace LogNormalizer.Tests;

public class Format2LogRecordParserTests
{
    private readonly Format2LogRecordParser _parser = new();

    [Fact]
    public void TryParse_ParsesValidLineWithCallerMethod()
    {
        string line = "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'";

        bool ok = _parser.TryParse(line, out var record);

        Assert.True(ok);
        Assert.Equal(new DateOnly(2025, 3, 10), record!.Date);
        Assert.Equal("15:14:51.5882", record.Time);
        Assert.Equal("INFO", record.Level);
        Assert.Equal("MobileComputer.GetDeviceId", record.CallerMethod);
        Assert.Equal("Код устройства: '@MINDEO-M40-D-410244015546'", record.Message);
    }

    [Fact]
    public void TryParse_UsesDefaultWhenCallerMethodMissing()
    {
        string line = "2025-03-10 15:14:51.5882| INFO|11|| Сообщение без вызвавшего метода";

        bool ok = _parser.TryParse(line, out var record);

        Assert.True(ok);
        Assert.Equal(LogRecord.DefaultCallerMethod, record!.CallerMethod);
    }

    [Theory]
    [InlineData("")]
    [InlineData("это не лог-строка вообще")]
    [InlineData("10.03.2025 15:14:49.523 INFORMATION Не тот формат")]
    [InlineData("2025-03-10 15:14:51.5882| TRACE|11|Caller| Неизвестный уровень")]
    public void TryParse_RejectsInvalidLines(string line)
    {
        bool ok = _parser.TryParse(line, out var record);

        Assert.False(ok);
        Assert.Null(record);
    }
}
