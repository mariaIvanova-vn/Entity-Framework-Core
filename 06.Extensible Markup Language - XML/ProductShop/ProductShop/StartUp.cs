using AutoMapper;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            string inputXml = File.ReadAllText("../../../Datasets/users.xml");

            string result = ImportUsers(context, inputXml);
            Console.WriteLine(result);
        }

        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();

            XmlHelper xmlHelper = new XmlHelper();
            ImportUsersDto[] userDtos = xmlHelper.Deserialize<ImportUsersDto[]>(inputXml, "Users");

            ICollection<User> users = new HashSet<User>();
            foreach (var item in userDtos)
            {          
                User user = mapper.Map<User>(item);
                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count}";
        }


        private static IMapper InitializeAutoMapper() =>
            new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
    }
}