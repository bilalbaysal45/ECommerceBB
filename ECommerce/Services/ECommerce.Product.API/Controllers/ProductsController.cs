using ECommerce.Product.API.Core.Application.Products.Commands.CreateProduct;
using ECommerce.Product.API.Core.Application.Products.Commands.DeleteProduct;
using ECommerce.Product.API.Core.Application.Products.Commands.UpdateProduct;
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
        public async Task<IActionResult> GetAll([FromQuery] GetProductsQuery query)
        {
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));

            if (!result)
                return NotFound("Silinecek ürün bulunamadı.");

            return NoContent(); // 204 No Content: İşlem başarılı ama dönecek veri yok
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
        {
            // Güvenlik: URL'deki Id ile Body içindeki Id uyuşmalı
            if (id != command.Id)
                return BadRequest("Id uyuşmazlığı.");

            var result = await _mediator.Send(command);

            if (!result)
                return NotFound("Güncellenecek ürün bulunamadı.");

            return Ok("Ürün başarıyla güncellendi.");
        }
    }
}
