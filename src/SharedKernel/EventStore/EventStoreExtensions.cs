using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using EventStore.ClientAPI;

using VShop.SharedKernel.EventSourcing;
using VShop.SharedKernel.Infrastructure.Domain;
using VShop.SharedKernel.Infrastructure.Helpers;

namespace VShop.SharedKernel.EventStore
{
    public static class EventStoreExtensions
    {
        public static Task AppendEvents
        (
            this IEventStoreConnection connection,
            string streamName,
            long version,
            params IDomainEvent[] events
        )
        {
            if (events == null || !events.Any()) return Task.CompletedTask;

            EventMetadata eventMetadata = new() { EffectiveTime = DateTime.UtcNow };

            EventData[] preparedEvents = events
                .Select(
                    @event =>
                        new EventData
                        (
                            GuidHelper.NewSequentialGuid(),
                            EventTypeMapper.ToName(@event.GetType()),
                            true,
                            Serialize(@event),
                            Serialize(eventMetadata)
                        )
                )
                .ToArray();

            return connection.AppendToStreamAsync
            (
                streamName,
                version,
                preparedEvents
            );
        }

        private static byte[] Serialize(object data) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }
}