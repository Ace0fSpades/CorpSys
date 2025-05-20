using Pr4.Logic;
using Pr4.Models;
using Pr4.Interfaces;
using Pr4.Core;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Serialization;

namespace Pr4.Server
{
    public class Server : IGameServer
    {
        // Основные параметры сервера и игры
        private int _maxPlayers;                                  // Максимальное количество игроков
        private readonly int _port;                               // Порт для подключения
        private BlockingCollection<TcpClient> _clients;           // Коллекция подключенных клиентов
        private ICodeBreakerService _codeBreaker;                 // Объект для работы с секретным кодом
        private RoundResult _roundResult;                         // Результаты текущего раунда
        private bool _gameRunning = false;                        // Флаг, указывающий на активность игры
        private string? _winner = null;                           // Имя победителя
        private readonly int _maxAttempts;                        // Максимальное количество попыток
        private int _currentPlayerIndex = 0;                      // Индекс текущего игрока
        private List<StreamWriter> _clientWriters = new();        // Список писателей для клиентов
        private List<string> _playerNames = new();                // Список имен игроков
        private TcpListener _listener;                            // Слушатель для подключений
        private CodeBreaker.CodeMode _codeMode;                   // Режим кода (цифры, буквы, смешанный)
        private readonly UserInputService _userInputService;      // Сервис пользовательского ввода
        private readonly GameSettings _gameSettings;              // Настройки игры

        public Server(UserInputService userInputService, GameSettings gameSettings)
        {
            _userInputService = userInputService;
            _gameSettings = gameSettings;
            _port = gameSettings.Port;
            _maxAttempts = gameSettings.MaxAttempts;
            _clients = new BlockingCollection<TcpClient>();
        }

        public async Task StartAsync()
        {
            // Запрос количества игроков
            _maxPlayers = _userInputService.RequestPlayerCount();

            // Запрос режима кода
            _codeMode = _userInputService.RequestCodeMode();

            // Запуск сервера
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            Console.WriteLine($"Сервер запущен на порту {_port}. Ожидание подключения {_maxPlayers} игроков.");

            // Основной цикл сервера
            while (true)
            {
                // Инициализация для нового цикла игры
                _clients = new BlockingCollection<TcpClient>();
                _clientWriters = new List<StreamWriter>();
                _playerNames = new List<string>();
                _gameRunning = false;
                _winner = null;

                // Ожидание подключения игроков
                while (_clients.Count < _maxPlayers)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine($"Клиент подключен: {client.Client.RemoteEndPoint}");
                    _clients.Add(client);

                    NetworkStream stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    _clientWriters.Add(writer);
                    _playerNames.Add($"Игрок {client.Client.RemoteEndPoint}");

                    if (_maxPlayers - _clients.Count > 0)
                    {
                        await writer.WriteLineAsync($"Добро пожаловать в Код-Мастер! Ожидание подключения еще {_maxPlayers - _clients.Count} игроков.");
                    }
                    else
                    {
                        await writer.WriteLineAsync($"Добро пожаловать в Код-Мастер");
                    }
                }

                Console.WriteLine("Начало нового раунда...");
                await PlayGameAsync();
            }
        }

        // Основная игровая логика
        private async Task PlayGameAsync()
        {
            // Начать новый раунд
            StartNewRound();
            await PlayRoundAsync();

            // Спросить клиентов, хотят ли они играть в новый раунд
            List<Task<bool>> clientTasks = new List<Task<bool>>();
            for (int i = 0; i < _clients.Count; i++)
            {
                clientTasks.Add(AskClientForNewRoundAsync(_clients.ElementAt(i), _clientWriters[i]));
            }

            await Task.WhenAll(clientTasks);

            // Проверка ответов и определение, кто остается в игре
            var clientsToKeep = new BlockingCollection<TcpClient>();
            var writersToKeep = new List<StreamWriter>();
            var namesToKeep = new List<string>();

            for (int i = 0; i < _clients.Count; i++)
            {
                if (clientTasks[i].Result)
                {
                    clientsToKeep.Add(_clients.ElementAt(i));
                    writersToKeep.Add(_clientWriters[i]);
                    namesToKeep.Add(_playerNames[i]);
                }
                else
                {
                    try
                    {
                        await _clientWriters[i].WriteLineAsync("Спасибо за игру! Отключение...");
                        _clientWriters[i].Close();
                        _clients.ElementAt(i).Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Ошибка при отключении клиента: {e.Message}");
                    }
                }
            }

            _clients = clientsToKeep;
            _clientWriters = writersToKeep;
            _playerNames = namesToKeep;

            // Сообщить оставшимся клиентам об ожидании остальных игроков
            if (_clients.Count > 0 && _clients.Count < _maxPlayers)
            {
                await BroadcastMessageAsync($"Ожидание подключения еще {_maxPlayers - _clients.Count} игроков перед началом нового раунда...");
            }

            // Ожидание подключения новых игроков, если не хватает
            while (_clients.Count < _maxPlayers)
            {
                Console.WriteLine($"Ожидание подключения еще {_maxPlayers - _clients.Count} игроков.");
                
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine($"Клиент подключен: {client.Client.RemoteEndPoint}");
                    
                    NetworkStream stream = client.GetStream();
                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                    
                    _clients.Add(client);
                    _clientWriters.Add(writer);
                    _playerNames.Add($"Игрок {client.Client.RemoteEndPoint}");

                    if (_maxPlayers - _clients.Count > 0)
                    {
                        await writer.WriteLineAsync($"Добро пожаловать в Код-Мастер! Ожидание подключения еще {_maxPlayers - _clients.Count} игроков.");
                        // Сообщение всем остальным игрокам
                        await BroadcastMessageAsync($"Новый игрок присоединился! Ожидание подключения еще {_maxPlayers - _clients.Count} игроков.");
                    }
                    else
                    {
                        await writer.WriteLineAsync($"Добро пожаловать в Код-Мастер! Все игроки подключены. Игра скоро начнется.");
                        await BroadcastMessageAsync($"Все игроки подключены. Начинаем новый раунд...");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Ошибка при подключении нового клиента: {e.Message}");
                    // Небольшая задержка перед повторной попыткой
                    await Task.Delay(1000);
                }
            }

            Console.WriteLine("Начало нового раунда...");
            await BroadcastMessageAsync("Новый раунд начинается прямо сейчас!");
            await Task.Delay(1000); // Небольшая задержка перед началом
            await PlayGameAsync();
        }

        // Спрашивает клиента, хочет ли он сыграть еще раз
        private async Task<bool> AskClientForNewRoundAsync(TcpClient client, StreamWriter writer)
        {
            try
            {
                await writer.WriteLineAsync("Хотите сыграть еще один раунд? (y)es/(n)o");
                NetworkStream stream = client.GetStream();
                
                // Используем существующий поток, чтобы избежать проблем
                StreamReader reader = new StreamReader(stream, Encoding.UTF8, false, 1024, true);
                
                string answer = await reader.ReadLineAsync();
                bool wantsToContinue = answer?.ToLower() == "y";
                
                if (!wantsToContinue)
                {
                    Console.WriteLine($"Игрок с адреса {((IPEndPoint)client.Client.RemoteEndPoint).Address} покидает игру.");
                }
                else
                {
                    Console.WriteLine($"Игрок с адреса {((IPEndPoint)client.Client.RemoteEndPoint).Address} хочет продолжить игру.");
                }
                
                return wantsToContinue;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Исключение в AskClientForNewRoundAsync: {e.Message}");
                return false; // Предполагаем "нет" при ошибке
            }
        }

        // Начало нового раунда
        private void StartNewRound()
        {
            // Создание нового кода с выбранным режимом
            _codeBreaker = new CodeBreaker(codeLength: 4, _codeMode);
            _roundResult = new RoundResult
            {
                StartTime = DateTime.Now,
                SecretCode = _codeBreaker.GetSecretCode(),
                Guesses = new List<Guess>()
            };
            _gameRunning = true;
            _winner = null;
            _currentPlayerIndex = 0;

            Console.WriteLine($"Создан новый секретный код: {_codeBreaker.GetSecretCode()}");
        }

        // Проведение раунда игры
        private async Task PlayRoundAsync()
        {
            // Сообщить всем клиентам о начале игры
            await BroadcastMessageAsync($"Новая игра начинается! Секретный код состоит из {_codeBreaker.GetSecretCode().Length} символов.");
            await BroadcastMessageAsync($"Допустимые символы: {_codeBreaker.GetAllowedCharacters()}");
            await BroadcastMessageAsync("Черный маркер (B) означает правильный символ на правильной позиции.");
            await BroadcastMessageAsync("Белый маркер (W) означает правильный символ на неправильной позиции.");

            // Запуск задач для обработки клиентов
            var clientTasks = new List<Task>();
            for (int i = 0; i < _clients.Count; i++)
            {
                clientTasks.Add(HandleClientAsync(_clients.ElementAt(i), i));
            }

            await Task.WhenAll(clientTasks);

            // Сохранение результатов раунда
            _roundResult.EndTime = DateTime.Now;
            _roundResult.Winner = _winner;
            SaveRoundResult(_roundResult);
        }

        // Обработка взаимодействия с конкретным клиентом
        private async Task HandleClientAsync(TcpClient client, int clientIndex)
        {
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            StreamWriter writer = _clientWriters[clientIndex];

            try
            {
                int attemptCount = 1;
                bool solved = false;

                // Клиентский цикл догадок
                while (attemptCount <= _maxAttempts && _gameRunning && !solved)
                {
                    if (_currentPlayerIndex == clientIndex)
                    {
                        // Сообщение всем о текущем ходе
                        await BroadcastTurnMessageAsync();
                        
                        // Запрос догадки у текущего игрока
                        await writer.WriteLineAsync($"Ваш ход. Попытка {attemptCount} из {_maxAttempts}");
                        await writer.WriteLineAsync("Введите вашу догадку:");
                        
                        try
                        {
                            string guess = await reader.ReadLineAsync();
                            
                            // Проверка на отключение
                            if (guess == null)
                            {
                                _gameRunning = false;
                                await BroadcastMessageAsync($"Игрок {_playerNames[clientIndex]} отключился. Игра прервана.");
                                return;
                            }

                            // Проверка догадки
                            var (black, white) = _codeBreaker.CheckGuess(guess);
                            
                            // Сохранение догадки
                            _roundResult.Guesses.Add(new Guess
                            {
                                PlayerName = _playerNames[clientIndex],
                                Code = guess.ToUpper(),
                                Attempts = attemptCount
                            });

                            // Сообщение всем игрокам о результате
                            await BroadcastMessageAsync($"Догадка игрока {_playerNames[clientIndex]}: {guess.ToUpper()}");
                            await BroadcastMessageAsync($"Результат: {black} черных, {white} белых");

                            // Проверка на победу
                            if (black == _codeBreaker.GetSecretCode().Length)
                            {
                                // Победа!
                                _winner = _playerNames[clientIndex];
                                await BroadcastMessageAsync($"Игрок {_winner} угадал код {_codeBreaker.GetSecretCode()}! Игра завершена.");
                                _gameRunning = false;
                                solved = true;
                            }
                            else
                            {
                                // Передача хода следующему игроку
                                _currentPlayerIndex = (_currentPlayerIndex + 1) % _clients.Count;
                                
                                // Увеличение счетчика попыток если все игроки сделали ход
                                if (_currentPlayerIndex == 0)
                                {
                                    attemptCount++;
                                    
                                    // Проверка на исчерпание попыток
                                    if (attemptCount > _maxAttempts)
                                    {
                                        await BroadcastMessageAsync($"Исчерпаны все попытки! Никто не угадал код. Правильный код: {_codeBreaker.GetSecretCode()}");
                                        _gameRunning = false;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Ошибка при обработке догадки: {e.Message}");
                            // Передача хода следующему игроку при ошибке
                            _currentPlayerIndex = (_currentPlayerIndex + 1) % _clients.Count;
                        }
                    }
                    else
                    {
                        // Ожидание своего хода
                        await Task.Delay(100);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка обработки клиента {clientIndex}: {e.Message}");
                
                // Сообщение об отключении игрока
                string playerName = _playerNames[clientIndex];
                await BroadcastMessageAsync($"Игрок {playerName} отключился.");
                
                // При отключении игрока приостановка игры
                if (_gameRunning)
                {
                    _gameRunning = false;
                    await BroadcastMessageAsync("Игра прервана из-за отключения игрока.");
                }
            }
        }

        // Рассылка сообщения всем подключенным клиентам
        private async Task BroadcastMessageAsync(string message)
        {
            for (int i = 0; i < _clientWriters.Count; i++)
            {
                try
                {
                    await _clientWriters[i].WriteLineAsync(message);
                }
                catch (Exception) { /* Игнорируем ошибки при отправке */ }
            }
        }

        // Рассылка сообщения о текущем ходе
        private async Task BroadcastTurnMessageAsync()
        {
            for (int i = 0; i < _clients.Count; i++)
            {
                try
                {
                    if (i == _currentPlayerIndex)
                    {
                        await _clientWriters[i].WriteLineAsync("Ваш ход!");
                    }
                    else
                    {
                        await _clientWriters[i].WriteLineAsync($"Ход игрока {_playerNames[_currentPlayerIndex]}");
                    }
                }
                catch (Exception) { /* Игнорируем ошибки при отправке */ }
            }
        }

        // Сохранение результатов раунда в XML файл
        private void SaveRoundResult(RoundResult result)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(RoundResult));
                string filename = $"round_{DateTime.Now:yyyyMMdd_HHmmss}.xml";
                
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    serializer.Serialize(writer, result);
                }
                
                Console.WriteLine($"Результаты раунда сохранены в файл {filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при сохранении результатов: {e.Message}");
            }
        }
    }
}