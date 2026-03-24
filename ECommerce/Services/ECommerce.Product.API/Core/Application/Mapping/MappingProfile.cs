using AutoMapper;
using ECommerce.Product.API.Core.Application.Dtos;
using ECommerce.Product.API.Core.Domain.Entities;

namespace ECommerce.Product.API.Core.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Entities.Product, ProductDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<Domain.Entities.Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
    }
}
