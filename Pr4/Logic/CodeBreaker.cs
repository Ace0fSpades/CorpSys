using Pr4.Interfaces;

namespace Pr4.Logic
{
    public class CodeBreaker : ICodeBreakerService
    {
        private readonly string _secretCode;          // Секретный код
        private readonly int _codeLength;             // Длина кода
        private readonly string _allowedCharacters;   // Допустимые символы
        private readonly Random _random = new Random();
        
        // Режимы генерации кода
        public enum CodeMode
        {
            Digits,         // Только цифры
            AlphaNumeric,   // Буквы и цифры
            Letters         // Только буквы
        }

        // Конструктор с параметрами длины кода и режима
        public CodeBreaker(int codeLength = 4, CodeMode mode = CodeMode.Digits)
        {
            _codeLength = codeLength;
            
            // Выбор допустимых символов в зависимости от режима
            switch (mode)
            {
                case CodeMode.Digits:
                    _allowedCharacters = "0123456789";
                    break;
                case CodeMode.Letters:
                    _allowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case CodeMode.AlphaNumeric:
                    _allowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                default:
                    _allowedCharacters = "0123456789";
                    break;
            }
            
            // Генерация секретного кода
            _secretCode = GenerateSecretCode(codeLength);
        }

        // Метод генерации случайного кода заданной длины
        private string GenerateSecretCode(int codeLength)
        {
            char[] code = new char[codeLength];
            
            for (int i = 0; i < codeLength; i++)
            {
                int randomIndex = _random.Next(0, _allowedCharacters.Length);
                code[i] = _allowedCharacters[randomIndex];
            }
            
            return new string(code);
        }

        // Проверка догадки и возврат количества черных и белых маркеров
        public (int Black, int White) CheckGuess(string guess)
        {
            int black = 0;      // Количество черных маркеров (правильный символ на правильной позиции)
            int white = 0;      // Количество белых маркеров (правильный символ на неправильной позиции)
            List<int> usedSecret = new List<int>();   // Использованные индексы секретного кода
            List<int> usedGuess = new List<int>();    // Использованные индексы догадки

            // Приведение введенного текста к верхнему регистру
            guess = guess.ToUpper();

            // Проверка на черные маркеры (точное совпадение)
            for (int i = 0; i < _codeLength; i++)
            {
                if (i < guess.Length && guess[i] == _secretCode[i])
                {
                    black++;
                    usedSecret.Add(i);
                    usedGuess.Add(i);
                }
            }

            // Проверка на белые маркеры (символ есть, но не на своей позиции)
            for (int i = 0; i < _codeLength && i < guess.Length; i++)
            {
                if (!usedGuess.Contains(i))
                {
                    for (int j = 0; j < _codeLength; j++)
                    {
                        if (!usedSecret.Contains(j) && guess[i] == _secretCode[j])
                        {
                            white++;
                            usedSecret.Add(j);
                            usedGuess.Add(i);
                            break;
                        }
                    }
                }
            }

            return (black, white);
        }

        // Получение секретного кода (для отладки и завершения игры)
        public string GetSecretCode()
        {
            return _secretCode;
        }
        
        // Получение строки допустимых символов
        public string GetAllowedCharacters()
        {
            return _allowedCharacters;
        }
    }
}