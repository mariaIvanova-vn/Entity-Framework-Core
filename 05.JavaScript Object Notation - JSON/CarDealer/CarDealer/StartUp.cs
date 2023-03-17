using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new CarDealerContext();

            //string inputJson = File.ReadAllText(@"../../../Datasets/sales.json");

            string result = GetOrderedCustomers(context);
            Console.WriteLine(result);
        }

        //2.	Import Data
        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var mapper = new Mapper(new MapperConfiguration(m =>
            {
                m.AddProfile<CarDealerProfile>();
            }));

            ImportSuppliersDto[] importSuppliersDtos = JsonConvert.DeserializeObject<ImportSuppliersDto[]>(inputJson);

            var importSuppliers = new HashSet<Supplier>();
            foreach (var importDto in importSuppliersDtos)
            {
                var user = mapper.Map<Supplier>(importDto);

                importSuppliers.Add(user);
            }
            context.Suppliers.AddRange(importSuppliers);
            context.SaveChanges();

            return $"Successfully imported {importSuppliers.Count}.";
        }


        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            //var mapper = new Mapper(new MapperConfiguration(m =>
            //{
            //    m.AddProfile<CarDealerProfile>();
            //}));

            //ImportPartsDto[] importPartsDtos = JsonConvert.DeserializeObject<ImportPartsDto[]>(inputJson);

            //var parts = new HashSet<Part>();
            //foreach (var importDto in importPartsDtos)
            //{
            //    var user = mapper.Map<Part>(importDto);

            //    parts.Add(user);
            //}

            //context.Parts.AddRange(parts);
            //context.SaveChanges();
            //return $"Successfully imported {parts.Count}.";

            int[] suppliersIds = context.Suppliers.Select(x => x.Id).ToArray();

            var parts = JsonConvert
                .DeserializeObject<List<Part>>(inputJson)
                .Where(p => suppliersIds.Contains(p.SupplierId))
                .ToList();

            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}.";
        }


        //Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            //var mapper = new Mapper(new MapperConfiguration(m =>
            //{
            //    m.AddProfile<CarDealerProfile>();
            //}));
            //ImportCarsDto[] importCarsDtos = JsonConvert.DeserializeObject<ImportCarsDto[]>(inputJson);
            //var cars = new HashSet<Car>();
            //foreach (var item in importCarsDtos)
            //{
            //    var car = mapper.Map<Car>(item);
            //    cars.Add(car);
            //}
            //context.Cars.AddRange(cars);
            //context.SaveChanges();
            //return $"Successfully imported {cars.Count}.";

            var carsAndPartsDTO = JsonConvert.DeserializeObject<List<ImportCarsDto>>(inputJson);

            List<PartCar> parts = new List<PartCar>();
            List<Car> cars = new List<Car>();

            foreach (var dto in carsAndPartsDTO)
            {
                Car car = new Car()
                {
                    Make = dto.Make,
                    Model = dto.Model,
                    TravelledDistance = dto.TraveledDistance
                };
                cars.Add(car);

                foreach (var part in dto.PartsId.Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        Car = car,
                        PartId = part,
                    };
                    parts.Add(partCar);
                }
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {cars.Count}.";
        }

        //Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count}.";
        }


        //Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count}.";
        }


        //3.	Export Data
        //Query 14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var orderedCustomers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString(@"dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert.SerializeObject(orderedCustomers, Formatting.Indented);
        }

    }
}