using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Globalization;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //Supplier
            this.CreateMap<ImportSupplierDto, Supplier>();

            //Part
            this.CreateMap<ImportPartDto, Part>()
                .ForMember(d => d.SupplierId, opt => opt.MapFrom(s => s.SupplierId.Value));

            //Car
            this.CreateMap<ImportCarDto, Car>().ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());
            this.CreateMap<Car, ExportCarsDto>();
            this.CreateMap<Car, ExportCarsBmwDto>();

            //Customer
            this.CreateMap<ImportCustomersDto, Customer>().ForMember(d=>d.BirthDate, 
                opt=>opt.MapFrom(s=>DateTime.Parse(s.BirthDate,CultureInfo.InvariantCulture)));
            this.CreateMap<Supplier, ExportSuppliersDto>()
                 .ForMember(d => d.PartsCount, opt => opt.MapFrom(d => d.Parts.Count));

            //Sale
            this.CreateMap<ImportSalesDto, Sale>()
                .ForMember(d=>d.CarId,opt=>opt.MapFrom(s=>s.CarId.Value));
        }
    }
}
