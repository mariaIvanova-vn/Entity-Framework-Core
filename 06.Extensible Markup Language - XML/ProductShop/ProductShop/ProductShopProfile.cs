using AutoMapper;
using ProductShop.DTOs.Export;
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
            
            this.CreateMap<User, ExportUserSoldProductsDto>()
                .ForMember(d=>d.Products,opt => opt.MapFrom(s => s.ProductsSold.ToArray()));       

            //Products
            this.CreateMap<ImportProductsDto, Product>();
            this.CreateMap<Product, ExportProductsInRangeDto>()
                 .ForMember(d => d.BuyerFullName, opt =>
                opt.MapFrom(s => $"{s.Buyer.FirstName} {s.Buyer.LastName}"));
            this.CreateMap<Product, ExportSoldProductsDto>();


            //Category
            this.CreateMap<ImportCategoriesDto, Category>();
            this.CreateMap<Category, ExportCategoryDto>()
                .ForMember(d => d.Count, opt => opt.MapFrom(s => s.CategoryProducts.Count()))
                .ForMember(d => d.AveragePrice, opt => opt.MapFrom(s => s.CategoryProducts.Select(cp => cp.Product.Price).Average()))
                .ForMember(d => d.TotalRevenue, opt => opt.MapFrom(s => s.CategoryProducts.Select(cp => cp.Product.Price).Sum()));

            //CategoryProduct
            this.CreateMap<ImportCategoryProductsDto, CategoryProduct>();
        }
    }
}
