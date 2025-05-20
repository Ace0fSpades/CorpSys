using Pr2.Models;

namespace Pr2.Services
{
    /// <summary>
    /// Интерфейс для анализатора файлов
    /// </summary>
    public interface IFileAnalyzer
    {
        /// <summary>
        /// Асинхронно анализирует содержимое файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>Результат анализа файла</returns>
        Task<FileAnalysisResult> AnalyzeFileAsync(string filePath);
    }
} 