using ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder;
using ECommerce.Order.API.Core.Application.Orders.Queries.GetAllOrders;
using ECommerce.Order.API.Core.Application.Orders.Queries.GetOrderById;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetOrderById(id));
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            foreach (var header in Request.Headers)
            {
                Console.WriteLine($"{header.Key}: {header.Value}");
            }
            // Gateway'den (Ocelot) gelen Header'ı oku
            var buyerId = Request.Headers["X-User-Id"].ToString();
            var buyerId1 = Request.Headers.FirstOrDefault(h => h.Key.ToLower() == "x-user-id").Value.ToString();

            var result = await _mediator.Send(new GetAllOrdersQuery(buyerId));
            return Ok(result);
        }
    }
}
