using System.Text;
using LogNormalizer;

string inputPath = args.Length > 0 ? args[0] : PromptForInputPath();

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Файл не найден: {inputPath}");
    return 1;
}

string directory = Path.GetDirectoryName(Path.GetFullPath(inputPath))!;
string outputPath = args.Length > 1 ? args[1] : Path.Combine(directory, "output.txt");
string problemsPath = Path.Combine(directory, "problems.txt");

var normalizer = new LogFileNormalizer(new ILogRecordParser[]
{
    new Format1LogRecordParser(),
    new Format2LogRecordParser(),
});

var utf8NoBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

using (var input = new StreamReader(inputPath, Encoding.UTF8))
using (var output = new StreamWriter(outputPath, false, utf8NoBom))
using (var problems = new StreamWriter(problemsPath, false, utf8NoBom))
{
    var result = normalizer.Normalize(input, output, problems);

    Console.WriteLine($"Обработано записей: {result.ValidCount + result.InvalidCount}");
    Console.WriteLine($"  корректных: {result.ValidCount} -> {outputPath}");
    Console.WriteLine($"  невалидных: {result.InvalidCount} -> {problemsPath}");
}

return 0;

static string PromptForInputPath()
{
    Console.WriteLine("Введите путь к входному лог-файлу");
    return Console.ReadLine() ?? string.Empty;
}
