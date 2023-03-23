using AutoMapper;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //Users
            this.CreateMap<ImportUsersDto, User>()
               .ForMember(d => d.Age, opt => opt.MapFrom(s => s.Age.Value));
        }
    }
}
