using LogNormalizer;
using Xunit;

namespace LogNormalizer.Tests;

public class LogLevelNormalizerTests
{
    [Theory]
    [InlineData("INFORMATION", "INFO")]
    [InlineData("INFO", "INFO")]
    [InlineData("WARNING", "WARN")]
    [InlineData("WARN", "WARN")]
    [InlineData("ERROR", "ERROR")]
    [InlineData("DEBUG", "DEBUG")]
    [InlineData("information", "INFO")]
    public void TryNormalize_RecognizesKnownLevels(string raw, string expected)
    {
        bool ok = LogLevelNormalizer.TryNormalize(raw, out string normalized);

        Assert.True(ok);
        Assert.Equal(expected, normalized);
    }

    [Theory]
    [InlineData("TRACE")]
    [InlineData("")]
    [InlineData("VERBOSE")]
    public void TryNormalize_RejectsUnknownLevels(string raw)
    {
        bool ok = LogLevelNormalizer.TryNormalize(raw, out _);

        Assert.False(ok);
    }
}
