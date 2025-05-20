namespace Pr2.Models
{
    /// <summary>
    /// Модель для хранения результатов анализа файла
    /// </summary>
    public class FileAnalysisResult
    {
        public string FileName { get; set; }
        public int WordCount { get; set; }
        public int CharCount { get; set; }

        public FileAnalysisResult(string fileName, int wordCount, int charCount)
        {
            FileName = fileName;
            WordCount = wordCount;
            CharCount = charCount;
        }
    }
} 