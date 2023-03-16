using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var context = new CarDealerContext();

            string inputJson = File.ReadAllText(@"../../../Datasets/suppliers.json");

            string result = ImportSuppliers(context, inputJson);
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

    }
}