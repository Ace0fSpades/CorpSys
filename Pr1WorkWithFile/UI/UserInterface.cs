using System;
using System.IO;

namespace Pr1WorkWithFile.UI
{
    /// <summary>
    /// Класс для работы с пользовательским интерфейсом
    /// </summary>
    public class UserInterface
    {
        /// <summary>
        /// Запрашивает у пользователя путь к файлу
        /// </summary>
        /// <returns>Полный путь к файлу</returns>
        public string GetFilePath()
        {
            Console.Write("Введите путь к текстовому файлу: ");
            string workingDirectory = Directory.GetCurrentDirectory();
            return $"{workingDirectory}\\" + Console.ReadLine();
        }

        /// <summary>
        /// Запрашивает у пользователя слово для поиска
        /// </summary>
        /// <returns>Слово для поиска</returns>
        public string GetSearchWord()
        {
            Console.Write("Введите слово для поиска: ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Выводит результаты анализа текста
        /// </summary>
        /// <param name="totalWordCount">Общее количество слов</param>
        /// <param name="searchWord">Искомое слово</param>
        /// <param name="wordCount">Количество вхождений искомого слова</param>
        public void DisplayResults(int totalWordCount, string searchWord, int wordCount)
        {
            Console.WriteLine($"Общее количество слов в файле: {totalWordCount}");
            Console.WriteLine($"Слово '{searchWord}' найдено {wordCount} раз.");
        }

        /// <summary>
        /// Выводит сообщение об ошибке
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public void DisplayError(string message)
        {
            Console.WriteLine($"Произошла ошибка: {message}");
        }

        /// <summary>
        /// Выводит сообщение о том, что файл не найден
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        public void DisplayFileNotFoundError(string filePath)
        {
            Console.WriteLine("Файл не существует.");
            Console.WriteLine(filePath);
        }

        /// <summary>
        /// Ожидает нажатия любой клавиши
        /// </summary>
        public void WaitForKeyPress()
        {
            Console.WriteLine("Нажмите любую клавишу для выхода.");
            Console.ReadKey();
        }
    }
} 