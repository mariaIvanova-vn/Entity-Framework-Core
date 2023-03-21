using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            string inputXml = File.ReadAllText("../../../Datasets/sales.xml");

            string result = ImportSales(context, inputXml);
            Console.WriteLine(result);
        }


        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();
            ImportSupplierDto[] supplierDtos = xmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

            ICollection<Supplier> suppliers = new HashSet<Supplier>();
            foreach (var item in supplierDtos)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }
                //Manual mapping without automapping
                //Supplier supplier = new Supplier()
                //{
                //    Name = item.Name,
                //    IsImporter = item.IsImporter
                //};

                Supplier supplier = mapper.Map<Supplier>(item);
                suppliers.Add(supplier);
            }
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportPartDto[] partsDtos = xmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");

            ICollection<Part> parts = new HashSet<Part>();
            foreach (var partDto in partsDtos)
            {
                if (string.IsNullOrEmpty(partDto.Name))
                {
                    continue;
                }
                if (!partDto.SupplierId.HasValue || 
                    !context.Suppliers.Any(s=>s.Id == partDto.SupplierId))
                {
                    continue;
                }
                Part part = mapper.Map<Part>(partDto);
                parts.Add(part);
            }
            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}";
        }


        //Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCarDto[] partsDtos = xmlHelper.Deserialize<ImportCarDto[]>(inputXml, "Cars");

            ICollection<Car> cars = new HashSet<Car>();
            foreach (var carDto in partsDtos)
            {
                if (string.IsNullOrEmpty(carDto.Make) || string.IsNullOrEmpty(carDto.Model))
                {
                    continue;
                }
                Car car = mapper.Map<Car>(carDto);
                ICollection<PartCar> partCars = new HashSet<PartCar>();
                foreach (var partCar in carDto.Parts.DistinctBy(p=>p.PartId))
                {
                    if (!context.Parts.Any(p=>p.Id == partCar.PartId))
                    {
                        continue;
                    }
                    PartCar partC = new PartCar()
                    {
                        PartId = partCar.PartId
                    };
                    car.PartsCars.Add(partC);

                }
                cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }


        //Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportCustomersDto[] customersDtos = xmlHelper.Deserialize<ImportCustomersDto[]>(inputXml, "Customers");

            ICollection<Customer> customers = new HashSet<Customer>();
            foreach (var customerDto in customersDtos)
            {
                if (string.IsNullOrEmpty(customerDto.Name) || string.IsNullOrEmpty(customerDto.BirthDate))
                {
                    continue;
                }
                Customer customer = mapper.Map<Customer>(customerDto);
                customers.Add(customer);
            }
            context.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count}";
        }


        //Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportSalesDto[] salesDtos = xmlHelper.Deserialize<ImportSalesDto[]>(inputXml, "Sales");

            ICollection<Sale> sales = new HashSet<Sale>();
            foreach (var saleDto in salesDtos)
            {
                if (!saleDto.CarId.HasValue || !context.Cars.Any(c=>c.Id == saleDto.CarId.Value))
                {
                    continue;
                }
                Sale sale = mapper.Map<Sale>(saleDto);
                sales.Add(sale);
            }
            context.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count}";
        }


        private static IMapper InitializeAutoMapper() =>
            new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));
    }
}