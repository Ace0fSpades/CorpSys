using System;
using Pr1WorkWithFile.Models;
using Pr1WorkWithFile.Services;
using Pr1WorkWithFile.UI;

namespace Pr1WorkWithFile
{
    /// <summary>
    /// Главный класс программы
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Текстовый процессор файлов");

            UserInterface ui = new UserInterface();
            string filePath = ui.GetFilePath();
            
            TextFileProcessor fileProcessor = new TextFileProcessor(filePath);
            
            if (!fileProcessor.FileExists())
            {
                ui.DisplayFileNotFoundError(filePath);
                ui.WaitForKeyPress();
                return;
            }

            string searchWord = ui.GetSearchWord();

            try
            {
                string fileContent = fileProcessor.ReadAllText();
                TextAnalyzer analyzer = new TextAnalyzer(fileContent);
                
                int totalWordCount = analyzer.GetTotalWordCount();
                int wordCount = analyzer.CountWordOccurrences(searchWord);
                
                ui.DisplayResults(totalWordCount, searchWord, wordCount);
            }
            catch (Exception e)
            {
                ui.DisplayError(e.Message);
            }

            ui.WaitForKeyPress();
        }
    }
}