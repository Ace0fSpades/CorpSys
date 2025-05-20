using System.Threading.Tasks;

namespace Pr4.Interfaces
{
    /// <summary>
    /// Интерфейс игрового клиента
    /// </summary>
    public interface IGameClient
    {
        /// <summary>
        /// Запускает клиент
        /// </summary>
        Task StartAsync();
    }
} 