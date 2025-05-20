using System;
using System.Text.RegularExpressions;

namespace Pr1WorkWithFile.Models
{
    /// <summary>
    /// Класс для анализа текста
    /// </summary>
    public class TextAnalyzer
    {
        private string content;

        public TextAnalyzer(string content)
        {
            this.content = content;
        }

        /// <summary>
        /// Получает общее количество слов в тексте
        /// </summary>
        /// <returns>Количество слов</returns>
        public int GetTotalWordCount()
        {
            string[] words = Regex.Split(content, @"\s+");
            return words.Length;
        }

        /// <summary>
        /// Подсчитывает количество вхождений определенного слова в тексте
        /// </summary>
        /// <param name="word">Слово для поиска</param>
        /// <returns>Количество вхождений слова</returns>
        public int CountWordOccurrences(string word)
        {
            string pattern = $@"\b{Regex.Escape(word)}\b";
            MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);
            return matches.Count;
        }
    }
} 