namespace Pr3_Server.Models
{
    public class FileAnalysisResult
    {
        public string FileName { get; set; } = string.Empty;
        public int LineCount { get; set; }
        public int WordCount { get; set; }
        public int CharCount { get; set; }

        public override string ToString()
        {
            return $"Имя файла: {FileName}\nСтрок: {LineCount}, Слов: {WordCount}, Символов: {CharCount}\n";
        }
    }
} 