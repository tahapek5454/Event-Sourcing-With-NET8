
using MongoDB.Driver;
using ProductEventHandlerService.Models;
using Shared;
using Shared.Events;
using Shared.Services.Abstract;
using System.Reflection;
using System.Text.Json;

namespace ProductEventHandlerService.Services
{
    internal class EventStoreServiceBG(IEventStoreService _eventStoreService, IMongoDbService _mongoDbService) : BackgroundService
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

                     var productCollection = _mongoDbService.GetCollection<Product>(ConstValues.ProductCollectionName);

                     switch (@event)
                     {
                         case NewProductAddedEvent e:
                             var hasProduct = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).AnyAsync();

                             if (hasProduct)
                                 break;

                             await productCollection.InsertOneAsync(new()
                             {
                                  Id = e.ProductId,
                                  Count = e.InitialCount,
                                  IsAvailable = e.IsAvailable,
                                  Price = e.InitialPrice,
                                  ProductName = e.ProductName,
                             });

                             break;

                         default:
                             break;
                     }
                 }
                );

        }
    }
}
