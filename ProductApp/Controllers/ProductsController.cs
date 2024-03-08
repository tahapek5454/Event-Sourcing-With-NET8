using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApp.Models.ViewModels;

namespace ProductApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductVM createProductVM)
        {

            return Ok();
        }
    }
}
