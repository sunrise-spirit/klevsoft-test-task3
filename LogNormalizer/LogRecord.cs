namespace LogNormalizer;

public sealed record LogRecord(DateOnly Date, string Time, string Level, string CallerMethod, string Message)
{
    public const string DefaultCallerMethod = "DEFAULT";

    public string ToOutputLine() =>
        string.Join('\t', Date.ToString("dd-MM-yyyy"), Time, Level, CallerMethod, Message);
}
