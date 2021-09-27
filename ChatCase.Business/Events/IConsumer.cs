using System.Threading.Tasks;

namespace ChatCase.Business.Events
{
    /// <summary>
    /// Consumer interface
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public interface IConsumer<T>
    {
        /// <summary>
        /// Handle event
        /// </summary>
        /// <param name="eventMessage"></param>
        /// <returns></returns>
        Task HandleEventAsync(T eventMessage);
    }
}
