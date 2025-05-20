using System;
using System.IO;

namespace Pr1WorkWithFile.Services
{
    /// <summary>
    /// Класс для обработки текстовых файлов
    /// </summary>
    public class TextFileProcessor
    {
        private string filePath;

        /// <summary>
        /// Инициализирует новый экземпляр класса TextFileProcessor
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        public TextFileProcessor(string filePath)
        {
            this.filePath = filePath;
        }

        /// <summary>
        /// Проверяет существование файла
        /// </summary>
        /// <returns>True, если файл существует; в противном случае — false</returns>
        public bool FileExists()
        {
            return File.Exists(filePath);
        }

        /// <summary>
        /// Читает весь текст из файла
        /// </summary>
        /// <returns>Содержимое файла</returns>
        public string ReadAllText()
        {
            return File.ReadAllText(filePath);
        }
    }
} 