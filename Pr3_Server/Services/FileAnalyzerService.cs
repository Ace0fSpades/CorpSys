using Microsoft.Extensions.Options;
using Pr3_Server.Models;

namespace Pr3_Server.Services
{
    public class FileAnalyzerService : IFileAnalyzerService
    {
        private static readonly object _fileLock = new object();
        private readonly ILogger<FileAnalyzerService> _logger;
        private readonly FileUploadOptions _options;

        public FileAnalyzerService(
            ILogger<FileAnalyzerService> logger,
            IOptions<FileUploadOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<FileAnalysisResult> AnalyzeFileAsync(string filePath)
        {
            int lineCount = 0;
            int wordCount = 0;
            int charCount = 0;

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        lineCount++;
                        charCount += line.Length;
                        wordCount += line.Split(new char[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    }
                }

                var fileName = Path.GetFileName(filePath);
                return new FileAnalysisResult
                {
                    FileName = fileName,
                    LineCount = lineCount,
                    WordCount = wordCount,
                    CharCount = charCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при анализе файла {FilePath}", filePath);
                throw;
            }
        }

        public void SaveAnalysisResult(string fileName, FileAnalysisResult result)
        {
            string analysisFilePath = Path.Combine(Directory.GetCurrentDirectory(), _options.AnalysisResultsFile);
            string resultEntry = $"Файл: {fileName}\n{result}\n";

            try
            {
                lock (_fileLock)
                {
                    File.AppendAllText(analysisFilePath, resultEntry);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении результата анализа для файла {FileName}", fileName);
                throw;
            }
        }
    }
} 