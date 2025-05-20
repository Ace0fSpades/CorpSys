namespace Pr4.Interfaces
{
    /// <summary>
    /// Интерфейс сервиса для игровой логики "Код-Мастер"
    /// </summary>
    public interface ICodeBreakerService
    {
        /// <summary>
        /// Проверяет догадку игрока
        /// </summary>
        /// <param name="guess">Текст догадки</param>
        /// <returns>Кортеж (черные маркеры, белые маркеры)</returns>
        (int Black, int White) CheckGuess(string guess);

        /// <summary>
        /// Возвращает секретный код
        /// </summary>
        string GetSecretCode();

        /// <summary>
        /// Возвращает допустимые символы для кода
        /// </summary>
        string GetAllowedCharacters();
    }
} 