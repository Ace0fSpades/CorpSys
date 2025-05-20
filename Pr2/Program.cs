using Pr2.Services;

namespace Pr2
{
    class Program
    {
        private static readonly string InputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input");

        static async Task Main(string[] args)
        {
            Console.WriteLine("Многопоточный анализатор файлов");

            // Создаем сервисы, используя внедрение зависимостей
            IFileReader fileReader = new FileReader();
            IFileAnalyzer fileAnalyzer = new TextFileAnalyzer(fileReader);
            FileAnalysisService analysisService = new FileAnalysisService(fileAnalyzer);

            // Запускаем анализ файлов
            await analysisService.AnalyzeFilesAsync(InputDirectory);

            // Выводим результаты
            Console.WriteLine("\nРезультаты анализа:");
            var results = analysisService.Results;
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {results[i].FileName}: {results[i].WordCount} слов, {results[i].CharCount} символов");
            }

            Console.WriteLine("\nИтог: {0} слов, {1} символов", analysisService.TotalWordCount, analysisService.TotalCharCount);

            Console.WriteLine("\nНажмите любую клавишу для выхода.");
            Console.ReadKey();
        }
    }
}