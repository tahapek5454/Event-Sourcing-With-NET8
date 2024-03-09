using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ProductApp.Models.ViewModels;
using Shared;
using Shared.Events;
using Shared.Services.Abstract;

namespace ProductApp.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController(IEventStoreService _eventStoreService, IMongoDbService _mongoDbService) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductVM createProductVM)
        {

            NewProductAddedEvent @event = new NewProductAddedEvent()
            {
                ProductId = Guid.NewGuid().ToString(),
                InitialCount = createProductVM.Count,
                InitialPrice = createProductVM.Price,
                IsAvailable = createProductVM.IsAvailable,
                ProductName = createProductVM.ProductName,
            };

            await _eventStoreService.AppendToStreamAsync(ConstValues.ProductsStream,
                    new[] { _eventStoreService.GenerateEventData(@event) }
                );

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCount([FromQuery] string id, [FromQuery] int value)
        {
            var collection = _mongoDbService.GetCollection<Shared.Models.Product>(ConstValues.ProductCollectionName);

            var product = await (await collection.FindAsync(p => p.Id.Equals(id))).FirstOrDefaultAsync();

            if(product.Count > value)
            {
                CountDecreasedEvent countDecreasedEvent = new()
                {
                    ProductId = product.Id,
                    DecrementAmount = product.Count - value,
                };

                await _eventStoreService.AppendToStreamAsync(ConstValues.ProductsStream, new[]
                {
                    _eventStoreService.GenerateEventData(countDecreasedEvent)
                });

            }
            else if(product.Count < value)
            {
                CountIncreasedEvent countIncreasedEvent = new()
                {
                    ProductId = product.Id,
                    IncrementAmount = value - product.Count,
                };

                await _eventStoreService.AppendToStreamAsync(ConstValues.ProductsStream, new[]
                {
                    _eventStoreService.GenerateEventData(countIncreasedEvent)
                });
            }

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePrice([FromQuery] string id ,[FromQuery] decimal value)
        {
            var collection = _mongoDbService.GetCollection<Shared.Models.Product>(ConstValues.ProductCollectionName);

            var product = await (await collection.FindAsync(p => p.Id.Equals(id))).FirstOrDefaultAsync();

            if (product.Price > value)
            {
                PriceDecreasedEvent priceDecreasedEvent = new()
                {
                    ProductId = product.Id,
                    DecrementPrice = product.Price - value,
                };

                await _eventStoreService.AppendToStreamAsync(ConstValues.ProductsStream, new[]
                {
                    _eventStoreService.GenerateEventData(priceDecreasedEvent)
                });
            }
            else if (product.Price < value)
            {
                PriceIncreasedEvent priceIncreasedEvent = new()
                {
                    ProductId = product.Id,
                    IncrementPrice = value - product.Price,
                };

                await _eventStoreService.AppendToStreamAsync(ConstValues.ProductsStream, new[]
                {
                    _eventStoreService.GenerateEventData(priceIncreasedEvent)
                });
            }

            return NoContent();

        }

        [HttpPut]
        public async Task<IActionResult> UpdateAvailable([FromQuery] string id, [FromQuery] bool value)
        {
            var collection = _mongoDbService.GetCollection<Shared.Models.Product>(ConstValues.ProductCollectionName);

            var product = await (await collection.FindAsync(p => p.Id.Equals(id))).FirstOrDefaultAsync();


            if (product.IsAvailable != value)
            {
                AvailabilityChangedEvent availabilityChangedEvent = new()
                {
                    IsAvailable = value,
                    ProductId = product.Id
                };
                await _eventStoreService.AppendToStreamAsync(ConstValues.ProductsStream, new[]
                {
                    _eventStoreService.GenerateEventData(availabilityChangedEvent)
                });
            }
      

            return NoContent();

        }

    }
}
