using System.Threading.Tasks;

namespace Pr4.Interfaces
{
    /// <summary>
    /// Интерфейс приложения
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Запускает приложение
        /// </summary>
        Task RunAsync();
    }
} 