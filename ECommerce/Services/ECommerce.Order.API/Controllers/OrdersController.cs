using ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder;
using ECommerce.Order.API.Models.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return result ? Ok("Sipariş oluşturuldu ve stok güncelleme mesajı gönderildi.")
                          : BadRequest("Sipariş oluşturulamadı.");
        }
    }
}
