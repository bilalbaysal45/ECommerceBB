namespace ECommerce.Stock.API.Core.Domain.Entities
{
    public class Stock
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int StockCount { get; set; } // Mevcut stok adedi
    }
}
