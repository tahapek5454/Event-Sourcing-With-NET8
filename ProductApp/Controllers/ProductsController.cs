using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApp.Models.ViewModels;
using Shared;
using Shared.Events;
using Shared.Services.Abstract;

namespace ProductApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IEventStoreService _eventStoreService) : ControllerBase
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
    }
}
