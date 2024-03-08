using EventStore.Client;
using Shared.Services.Abstract;
using System.Text.Json;

namespace Shared.Services.Concrete
{
    public class EventStoreService(string connectionString) : IEventStoreService
    {
        private readonly string _connectionString = connectionString;
        public async Task AppendToStreamAsync(string streamName, IEnumerable<EventData> eventData)
         => await Client.AppendToStreamAsync(
                streamName: streamName,
                eventData: eventData,
                expectedState: StreamState.Any
             );
        public EventData GenerateEventData(object @event)
            => new(
                    eventId: Uuid.NewUuid(),
                    type: @event.GetType().Name,
                    data: JsonSerializer.SerializeToUtf8Bytes(@event)
                );
        private EventStoreClientSettings GetEventStoreClientSettings(string connectionString)
            => EventStoreClientSettings.Create(connectionString);
        private EventStoreClient Client { get => new(GetEventStoreClientSettings(_connectionString)); }

    }
}
