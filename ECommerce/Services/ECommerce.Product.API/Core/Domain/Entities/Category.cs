namespace ECommerce.Product.API.Core.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation Property: Bir kategorinin birden fazla ürünü olabilir
        public ICollection<Product> Products { get; set; }
    }
}
