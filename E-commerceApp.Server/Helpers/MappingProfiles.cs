using AutoMapper;
using Core.Entities;
using E_commerceApp.Server.Dtos;

namespace E_commerceApp.Server.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, s => s.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, s => s.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, s => s.MapFrom<ProductUrlResolver>());
        }
    }
}
