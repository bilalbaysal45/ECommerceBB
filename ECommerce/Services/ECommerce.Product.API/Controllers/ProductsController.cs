using ECommerce.Product.API.Core.Application.Products.Commands.CreateProduct;
using ECommerce.Product.API.Core.Application.Products.Queries.GetProductById;
using ECommerce.Product.API.Core.Application.Products.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Product.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result); // Oluşturulan ürünün Id'sini döner
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetProductsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));

            if (result == null)
                return NotFound("Ürün bulunamadı.");

            return Ok(result);
        }
    }
}
