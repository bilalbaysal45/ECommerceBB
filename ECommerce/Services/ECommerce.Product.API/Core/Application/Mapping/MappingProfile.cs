using AutoMapper;
using ECommerce.Product.API.Core.Application.Dtos;

namespace ECommerce.Product.API.Core.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.Product, ProductDto>().ReverseMap();
        }
    }
}
