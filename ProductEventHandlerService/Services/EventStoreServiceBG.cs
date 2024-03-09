
using Shared;
using Shared.Events;
using Shared.Services.Abstract;
using System.Reflection;
using System.Text.Json;

namespace ProductEventHandlerService.Services
{
    internal class EventStoreServiceBG(IEventStoreService _eventStoreService) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _eventStoreService.SubscribeToStreamAsync
                (ConstValues.ProductsStream,
                 async (streamSubscription, resolvedEvent, cancellationToken) =>
                 {
                     string eventTypeText = resolvedEvent.Event.EventType;
                     object @event = JsonSerializer.Deserialize(resolvedEvent.Event.Data.ToArray(),
                         Assembly.Load("Shared").GetTypes().FirstOrDefault(t => t.Name.Equals(eventTypeText))
                         );

                     switch (@event)
                     {
                         case NewProductAddedEvent e:
                             break;

                         default:
                             break;
                     }
                 }
                );

        }
    }
}
