using Pr2.Models;

namespace Pr2.Services
{
    /// <summary>
    /// Сервис для анализа файлов
    /// </summary>
    public class FileAnalysisService
    {
        private readonly IFileAnalyzer _fileAnalyzer;
        private readonly List<FileAnalysisResult> _results;
        private readonly object _lock = new object();
        private int _totalWordCount = 0;
        private int _totalCharCount = 0;

        public FileAnalysisService(IFileAnalyzer fileAnalyzer)
        {
            _fileAnalyzer = fileAnalyzer;
            _results = new List<FileAnalysisResult>();
        }

        /// <summary>
        /// Получает результаты анализа
        /// </summary>
        public IReadOnlyList<FileAnalysisResult> Results => _results.AsReadOnly();

        /// <summary>
        /// Общее количество слов во всех проанализированных файлах
        /// </summary>
        public int TotalWordCount => _totalWordCount;

        /// <summary>
        /// Общее количество символов во всех проанализированных файлах
        /// </summary>
        public int TotalCharCount => _totalCharCount;

        /// <summary>
        /// Анализирует все файлы в указанной директории
        /// </summary>
        /// <param name="directoryPath">Путь к директории</param>
        /// <param name="searchPattern">Шаблон поиска файлов</param>
        /// <returns>Задача, представляющая асинхронную операцию</returns>
        public async Task AnalyzeFilesAsync(string directoryPath, string searchPattern = "*.txt")
        {
            // Проверяем существование директории
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Получаем список файлов
            string[] files = Directory.GetFiles(directoryPath, searchPattern);

            if (files.Length == 0)
            {
                Console.WriteLine($"В директории '{directoryPath}' не найдено файлов по шаблону '{searchPattern}'.");
                return;
            }

            // Обрабатываем файлы параллельно
            List<Task> tasks = new List<Task>();
            foreach (string file in files)
            {
                tasks.Add(ProcessFileAsync(file));
            }

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Обрабатывает файл и добавляет результаты в общий список
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        private async Task ProcessFileAsync(string filePath)
        {
            try
            {
                // Анализируем файл
                FileAnalysisResult result = await _fileAnalyzer.AnalyzeFileAsync(filePath);

                // Добавляем результаты в список, используя блокировку
                lock (_lock)
                {
                    _results.Add(result);
                    _totalWordCount += result.WordCount;
                    _totalCharCount += result.CharCount;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при обработке файла {filePath}: {e.Message}");
            }
        }
    }
} 