namespace Pr2.Services
{
    /// <summary>
    /// Реализация чтения файлов
    /// </summary>
    public class FileReader : IFileReader
    {
        /// <summary>
        /// Асинхронно читает содержимое файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>Содержимое файла</returns>
        public async Task<string> ReadFileAsync(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при чтении файла {filePath}: {e.Message}");
                return string.Empty;
            }
        }
    }
} 