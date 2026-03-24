using ECommerce.Product.API.Core.Application.Dtos;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Products.Queries.GetProducts
{
    // List<Product> dönecek bir sorgu tanımlıyoruz
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public string? SearchTerm { get; set; } // İsimde arama yapmak için
        public Guid? CategoryId { get; set; }  // Belirli bir kategoriyi süzmek için
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
