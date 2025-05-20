using System.Xml.Serialization;

namespace Pr4.Models
{
    // Класс для хранения результатов раунда игры (сериализуется в XML)
    [XmlRoot("RoundResult")]
    public class RoundResult
    {
        public RoundResult()
        {
            // Инициализация списка при создании объекта
            Guesses = new List<Guess>();
        }

        public DateTime StartTime { get; set; }    // Время начала раунда
        public DateTime EndTime { get; set; }      // Время окончания раунда
        public string SecretCode { get; set; } = string.Empty;     // Секретный код раунда

        // Список всех догадок игроков в этом раунде
        [XmlArray("Guesses")]
        [XmlArrayItem("Guess")]
        public List<Guess> Guesses { get; set; }

        public string Winner { get; set; } = string.Empty;         // Имя победителя (если есть)
    }

    // Класс для хранения информации об отдельной догадке игрока
    public class Guess
    {
        public string PlayerName { get; set; } = string.Empty;     // Имя игрока
        public string Code { get; set; } = string.Empty;           // Предложенный код
        public int Attempts { get; set; }          // Номер попытки
    }
}