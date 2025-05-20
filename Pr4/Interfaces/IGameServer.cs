using System.Threading.Tasks;

namespace Pr4.Interfaces
{
    /// <summary>
    /// Интерфейс игрового сервера
    /// </summary>
    public interface IGameServer
    {
        /// <summary>
        /// Запускает сервер
        /// </summary>
        Task StartAsync();
    }
} 