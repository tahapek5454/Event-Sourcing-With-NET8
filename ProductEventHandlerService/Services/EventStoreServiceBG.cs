
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Shared.Models;
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

                     Product product = null;
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

                         case CountDecreasedEvent e:
                             product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                             if(product is not null)
                             {
                                 product.Count -= e.DecrementAmount;
                                 await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                             }
                             break;

                         case CountIncreasedEvent e:
                             product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                             if (product is not null)
                             {
                                 product.Count += e.IncrementAmount;
                                 await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                             }
                             break;

                         case PriceDecreasedEvent e:
                             product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                             if (product is not null)
                             {
                                 product.Price -= e.DecrementPrice;
                                 await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                             }
                             break;

                         case PriceIncreasedEvent e:
                             product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                             if (product is not null)
                             {
                                 product.Price += e.IncrementPrice;
                                 await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                             }
                             break;
                         case AvailabilityChangedEvent e:
                             product = await (await productCollection.FindAsync(p => p.Id == e.ProductId)).FirstOrDefaultAsync();
                             if (product is not null)
                             {
                                product.IsAvailable = e.IsAvailable;
                                await productCollection.FindOneAndReplaceAsync(p => p.Id == e.ProductId, product);
                             }
                             break;

                         default:
                             break;
                     }
                 }
                );

        }
    }
}
