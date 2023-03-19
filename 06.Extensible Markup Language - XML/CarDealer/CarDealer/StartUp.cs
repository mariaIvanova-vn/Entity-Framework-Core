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
            string inputXml = File.ReadAllText("../../../Datasets/suppliers.xml");

            string result = ImportSuppliers(context, inputXml);
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

        private static IMapper InitializeAutoMapper() =>
            new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));
    }
}