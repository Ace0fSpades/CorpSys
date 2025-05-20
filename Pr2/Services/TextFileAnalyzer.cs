using Pr2.Models;
using System.Text.RegularExpressions;

namespace Pr2.Services
{
    /// <summary>
    /// Анализатор текстовых файлов
    /// </summary>
    public class TextFileAnalyzer : IFileAnalyzer
    {
        private readonly IFileReader _fileReader;

        public TextFileAnalyzer(IFileReader fileReader)
        {
            _fileReader = fileReader;
        }

        /// <summary>
        /// Асинхронно анализирует текстовый файл
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        /// <returns>Результат анализа файла</returns>
        public async Task<FileAnalysisResult> AnalyzeFileAsync(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            string fileContent = await _fileReader.ReadFileAsync(filePath);

            // Подсчитываем количество слов и символов
            int wordCount = CountWords(fileContent);
            int charCount = fileContent.Length;

            return new FileAnalysisResult(fileName, wordCount, charCount);
        }

        /// <summary>
        /// Подсчитывает количество слов в тексте
        /// </summary>
        private int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            return Regex.Split(text, @"\s+").Length;
        }
    }
} 