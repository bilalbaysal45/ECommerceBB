namespace ECommerce.Order.API.Models.Dtos
{
    public class OrderCreateDto
    {
        public string UserId { get; set; }
        public List<OrderItemCreateDto> Items { get; set; }
    }
}
