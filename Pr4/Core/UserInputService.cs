using Pr4.Logic;

namespace Pr4.Core
{
    /// <summary>
    /// Сервис для работы с пользовательским вводом
    /// </summary>
    public class UserInputService
    {
        /// <summary>
        /// Запрашивает количество игроков у пользователя
        /// </summary>
        /// <param name="minPlayers">Минимальное количество игроков</param>
        /// <param name="maxPlayers">Максимальное количество игроков</param>
        /// <returns>Выбранное пользователем количество игроков</returns>
        public int RequestPlayerCount(int minPlayers = 2, int maxPlayers = 4)
        {
            int playerCount;
            Console.WriteLine($"Введите количество игроков ({minPlayers}-{maxPlayers}):");
            
            while (!int.TryParse(Console.ReadLine(), out playerCount) || playerCount < minPlayers || playerCount > maxPlayers)
            {
                Console.WriteLine($"Некорректное значение. Введите число от {minPlayers} до {maxPlayers}:");
            }
            
            return playerCount;
        }

        /// <summary>
        /// Запрашивает режим кода у пользователя
        /// </summary>
        /// <returns>Выбранный пользователем режим кода</returns>
        public CodeBreaker.CodeMode RequestCodeMode()
        {
            Console.WriteLine("Выберите режим кода:");
            Console.WriteLine("1 - Только цифры (0-9)");
            Console.WriteLine("2 - Только буквы (A-Z)");
            Console.WriteLine("3 - Смешанный режим (буквы и цифры)");
            
            int modeChoice;
            while (!int.TryParse(Console.ReadLine(), out modeChoice) || modeChoice < 1 || modeChoice > 3)
            {
                Console.WriteLine("Некорректное значение. Введите число от 1 до 3:");
            }

            switch (modeChoice)
            {
                case 1:
                    Console.WriteLine("Выбран режим: только цифры");
                    return CodeBreaker.CodeMode.Digits;
                case 2:
                    Console.WriteLine("Выбран режим: только буквы");
                    return CodeBreaker.CodeMode.Letters;
                case 3:
                    Console.WriteLine("Выбран режим: смешанный (буквы и цифры)");
                    return CodeBreaker.CodeMode.AlphaNumeric;
                default:
                    Console.WriteLine("Выбран режим по умолчанию: только цифры");
                    return CodeBreaker.CodeMode.Digits;
            }
        }

        /// <summary>
        /// Запрашивает режим работы приложения
        /// </summary>
        /// <returns>Выбранный режим: "S" - сервер, "C" - клиент</returns>
        public string RequestApplicationMode()
        {
            Console.WriteLine("Выберите режим: (S) - сервер или (C) - клиент");
            string? input = Console.ReadLine();
            return input?.ToUpper() ?? "";
        }
    }
} 