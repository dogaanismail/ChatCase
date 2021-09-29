using ChatCase.Business.Services.Logging;
using ChatCase.Core.Events;
using ChatCase.Core.Infrastructure;
using Newtonsoft.Json;
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
                        LoggerFactory.DatabaseLogManager().Error($"EventPublisher- PublishAsync error: {JsonConvert.SerializeObject(exception)}");
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
