using LogNormalizer;
using Xunit;

namespace LogNormalizer.Tests;

public class LogFileNormalizerTests
{
    private static LogFileNormalizer CreateNormalizer() =>
        new(new ILogRecordParser[] { new Format1LogRecordParser(), new Format2LogRecordParser() });

    [Fact]
    public void Normalize_ProducesExpectedOutputForBothFormats()
    {
        string input = string.Join('\n',
            "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'",
            "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'");

        using var reader = new StringReader(input);
        using var output = new StringWriter();
        using var problems = new StringWriter();

        var result = CreateNormalizer().Normalize(reader, output, problems);

        string[] outputLines = output.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Equal(2, result.ValidCount);
        Assert.Equal(0, result.InvalidCount);
        Assert.Equal("", problems.ToString());
        Assert.Equal("10-03-2025\t15:14:49.523\tINFO\tDEFAULT\tВерсия программы: '3.4.0.48729'", outputLines[0]);
        Assert.Equal("10-03-2025\t15:14:51.5882\tINFO\tMobileComputer.GetDeviceId\tКод устройства: '@MINDEO-M40-D-410244015546'", outputLines[1]);
    }

    [Fact]
    public void Normalize_SendsUnrecognizedLinesToProblems()
    {
        string input = string.Join('\n',
            "10.03.2025 15:14:49.523 INFORMATION Корректная запись",
            "это совершенно не лог-строка",
            "10.03.2025 15:14:49.523 TRACE Неизвестный уровень логирования");

        using var reader = new StringReader(input);
        using var output = new StringWriter();
        using var problems = new StringWriter();

        var result = CreateNormalizer().Normalize(reader, output, problems);

        Assert.Equal(1, result.ValidCount);
        Assert.Equal(2, result.InvalidCount);
        string[] problemLines = problems.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Assert.Equal("это совершенно не лог-строка", problemLines[0]);
        Assert.Equal("10.03.2025 15:14:49.523 TRACE Неизвестный уровень логирования", problemLines[1]);
    }

    [Fact]
    public void Normalize_SkipsBlankLines()
    {
        string input = string.Join('\n',
            "10.03.2025 15:14:49.523 INFORMATION Строка 1",
            "",
            "   ",
            "10.03.2025 15:14:50.000 INFORMATION Строка 2");

        using var reader = new StringReader(input);
        using var output = new StringWriter();
        using var problems = new StringWriter();

        var result = CreateNormalizer().Normalize(reader, output, problems);

        Assert.Equal(2, result.ValidCount);
        Assert.Equal(0, result.InvalidCount);
        Assert.Equal("", problems.ToString());
    }

    [Fact]
    public void Normalize_ReturnsEmptyResultForEmptyInput()
    {
        using var reader = new StringReader(string.Empty);
        using var output = new StringWriter();
        using var problems = new StringWriter();

        var result = CreateNormalizer().Normalize(reader, output, problems);

        Assert.Equal(0, result.ValidCount);
        Assert.Equal(0, result.InvalidCount);
    }
}
