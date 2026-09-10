namespace LogNormalizer;


/// Разбирает одну строку лог-файла определённого формата в <see cref="LogRecord"/>.
/// Возвращает false, если строка не соответствует формату или её уровень логирования не распознан.

public interface ILogRecordParser
{
    bool TryParse(string line, out LogRecord? record);
}
