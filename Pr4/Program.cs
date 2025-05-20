using Pr4.Core;
using Pr4.Interfaces;

namespace Pr4
{
    /// <summary>
    /// Основной класс приложения
    /// </summary>
    class Program : IApplication
    {
        private readonly UserInputService _userInputService;
        private readonly GameSettings _gameSettings;

        /// <summary>
        /// Конструктор класса Program
        /// </summary>
        /// <param name="userInputService">Сервис пользовательского ввода</param>
        /// <param name="gameSettings">Настройки игры</param>
        public Program(UserInputService userInputService, GameSettings gameSettings)
        {
            _userInputService = userInputService;
            _gameSettings = gameSettings;
        }

        /// <summary>
        /// Точка входа в приложение
        /// </summary>
        static async Task Main(string[] args)
        {
            // Создание экземпляра сервисов
            var userInputService = new UserInputService();
            var gameSettings = new GameSettings();

            // Создание экземпляра приложения
            var application = new Program(userInputService, gameSettings);

            // Запуск приложения
            await application.RunAsync();
        }

        /// <summary>
        /// Запускает приложение в соответствии с выбранным режимом
        /// </summary>
        public async Task RunAsync()
        {
            string mode = _userInputService.RequestApplicationMode();

            if (mode == "S")
            {
                // Создание и запуск сервера
                IGameServer server = new Server.Server(_userInputService, _gameSettings);
                await server.StartAsync();
            }
            else if (mode == "C")
            {
                // Создание и запуск клиента
                IGameClient client = new Client.GameClient(_gameSettings);
                await client.StartAsync();
            }
            else
            {
                Console.WriteLine("Некорректный режим. Запустите программу снова.");
            }
        }
    }
}