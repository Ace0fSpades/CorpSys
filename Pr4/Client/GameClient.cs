using Pr4.Core;
using Pr4.Interfaces;
using System.Net.Sockets;
using System.Text;

namespace Pr4.Client
{
    public class GameClient : IGameClient
    {
        private readonly string _serverAddress;  // Адрес сервера
        private readonly int _port;              // Порт для подключения
        private readonly GameSettings _gameSettings;

        public GameClient(GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            _serverAddress = gameSettings.ServerAddress;
            _port = gameSettings.Port;
        }

        public async Task StartAsync()
        {
            try
            {
                Console.WriteLine("Подключение к серверу...");
                TcpClient client = new TcpClient();
                await client.ConnectAsync(_serverAddress, _port);
                Console.WriteLine("Подключено к серверу!");

                // Создание потоков для чтения и записи
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                Console.WriteLine(await reader.ReadLineAsync()); // Приветственное сообщение

                // Основной цикл клиента
                while (true)
                {
                    try
                    {
                        string serverMessage = await reader.ReadLineAsync();

                        // Проверка на отключение сервера
                        if (serverMessage == null)
                        {
                            Console.WriteLine("Соединение с сервером было закрыто.");
                            break;
                        }

                        Console.WriteLine(serverMessage);

                        // Обработка запроса на ввод догадки
                        if (serverMessage.Contains("Введите вашу догадку:"))
                        {
                            Console.Write("Ваша догадка: ");
                            string guess = Console.ReadLine();
                            await writer.WriteLineAsync(guess);
                        }
                        // Обработка запроса на продолжение игры
                        else if (serverMessage.Contains("Хотите сыграть еще один раунд?"))
                        {
                            Console.Write("Сыграть еще раунд? (y)es/(n)o: ");
                            string answer = Console.ReadLine();

                            // По умолчанию "нет" если ввод пустой
                            if (string.IsNullOrEmpty(answer))
                            {
                                answer = "n";
                                Console.WriteLine("Ввод не получен, используется значение по умолчанию 'n'");
                            }

                            await writer.WriteLineAsync(answer);

                            if (answer.ToLower() != "y")
                            {
                                Console.WriteLine("Вы решили не продолжать. Отключение...");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Ожидание начала следующего раунда...");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ошибка связи с сервером: {e.Message}");
                        break;
                    }
                }

                Console.WriteLine("Отключено от сервера. Нажмите любую клавишу для выхода.");
                Console.ReadKey();

                // Очистка ресурсов
                try
                {
                    writer.Close();
                    reader.Close();
                    client.Close();
                }
                catch { /* Игнорируем ошибки при закрытии */ }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Исключение: {e.Message}");
                Console.WriteLine("Нажмите любую клавишу для выхода.");
                Console.ReadKey();
            }
        }
    }
}