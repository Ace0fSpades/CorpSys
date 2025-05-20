namespace Pr2.Services
{
    /// <summary>
    /// Интерфейс для чтения файлов
    /// </summary>
    public interface IFileReader
    {
        /// <summary>
        /// Асинхронно читает содержимое файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>Содержимое файла</returns>
        Task<string> ReadFileAsync(string filePath);
    }
} 