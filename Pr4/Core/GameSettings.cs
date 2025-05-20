using Pr4.Logic;

namespace Pr4.Core
{
    /// <summary>
    /// Класс для хранения настроек игры
    /// </summary>
    public class GameSettings
    {
        /// <summary>
        /// Максимальное количество игроков
        /// </summary>
        public int MaxPlayers { get; set; } = 2;

        /// <summary>
        /// Порт сервера
        /// </summary>
        public int Port { get; set; } = 12345;

        /// <summary>
        /// Адрес сервера
        /// </summary>
        public string ServerAddress { get; set; } = "127.0.0.1";

        /// <summary>
        /// Максимальное количество попыток
        /// </summary>
        public int MaxAttempts { get; set; } = 10;

        /// <summary>
        /// Режим кода (цифры, буквы, смешанный)
        /// </summary>
        public CodeBreaker.CodeMode CodeMode { get; set; } = CodeBreaker.CodeMode.Digits;

        /// <summary>
        /// Длина кода
        /// </summary>
        public int CodeLength { get; set; } = 4;
    }
} 