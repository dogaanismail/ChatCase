using ChatCase.Core.Events;
using ChatCase.Core.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatCase.Business.Events
{
    /// <summary>
    /// Represents the event publisher implementation
    /// </summary>
    public partial class EventPublisher : IEventPublisher
    {
        #region Methods

        /// <summary>
        /// Publish event to consumers
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public virtual async Task PublishAsync<TEvent>(TEvent @event)
        {
            var consumers = EngineContext.Current.ResolveAll<IConsumer<TEvent>>().ToList();

            foreach (var consumer in consumers)
            {
                try
                {
                    await consumer.HandleEventAsync(@event);
                }
                catch (Exception exception)
                {
                    try
                    {
                        //var logger = EngineContext.Current.Resolve<ILogService>();
                        //if (logger == null)
                        //    return;

                        //await logger.ErrorAsync(exception.Message, exception);
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion
    }
}
