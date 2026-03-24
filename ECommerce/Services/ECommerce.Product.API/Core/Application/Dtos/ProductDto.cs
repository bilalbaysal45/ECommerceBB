namespace ECommerce.Product.API.Core.Application.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
    }
}
