namespace ECommerce.Product.API.Core.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Ürün kodu (SKU) - Stok servisi ile haberleşirken bu kodu referans alabiliriz.
        public string Sku { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedDate { get; set; }

        public Guid CategoryId { get; set; } // Foreign Key
        public Category Category { get; set; } // Navigation Property
    }
}
