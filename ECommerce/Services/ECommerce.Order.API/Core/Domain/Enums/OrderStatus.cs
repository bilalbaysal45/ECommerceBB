namespace ECommerce.Order.API.Core.Domain.Enums
{
    public enum OrderStatus
    {
        Suspend,   // İlk oluşturulduğunda, onay bekliyor
        Completed, // Stok ve ödeme başarılı
        Fail       // Stok yetersiz veya ödeme başarısız
    }
}
