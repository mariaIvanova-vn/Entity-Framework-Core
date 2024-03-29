﻿using AutoMapper;
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

            string result = GetSalesWithAppliedDiscount(context);
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
                    TraveledDistance = dto.TraveledDistance
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


        //Query 15. Export Cars from Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsFromMakeToyota = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToArray();
            return JsonConvert.SerializeObject(carsFromMakeToyota, Formatting.Indented);
        }


        //Query 16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var localSuppliers = context.Suppliers
                .Where(s=>s.IsImporter==false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                }).ToArray();

            return JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);
        }


        //Query 17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithTheirListOfParts = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars
                        .Select(p => new
                        {
                            p.Part.Name,
                            Price = $"{p.Part.Price:f2}"
                        }).ToArray()
                })
                .ToArray();
            return JsonConvert.SerializeObject(carsWithTheirListOfParts, Formatting.Indented);
        }


        //Query 18. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customerSales = context.Customers
               .Where(c => c.Sales.Any())
               .Select(c => new
               {
                   fullName = c.Name,
                   boughtCars = c.Sales.Count(),
                   salePrices = c.Sales.SelectMany(x => x.Car.PartsCars.Select(x => x.Part.Price))
               })
               .ToArray();
            var totalSalesByCustomer = customerSales.Select(t => new
            {
                t.fullName,
                t.boughtCars,
                spentMoney = t.salePrices.Sum()
            })
            .OrderByDescending(t => t.spentMoney)
            .ThenByDescending(t => t.boughtCars)
            .ToArray();

            return JsonConvert.SerializeObject(totalSalesByCustomer, Formatting.Indented);
        }


        //Query 19. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesWithDiscount = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartsCars.Sum(p => p.Part.Price):f2}",
                    priceWithDiscount = $"{s.Car.PartsCars.Sum(p => p.Part.Price) * (1 - s.Discount / 100):f2}"
                })
                .ToArray();

            return JsonConvert.SerializeObject(salesWithDiscount, Formatting.Indented);
        }
    }
}