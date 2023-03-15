using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Collections.Generic;

namespace ProductShop
{
    public class StartUp
    {
        
        public static void Main()
        {
            var context = new ProductShopContext();
            string inputJson = File.ReadAllText(@"../../../Datasets/users.json");

            string result = ImportUsers(context, inputJson);
            Console.WriteLine(result);
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var mapper = new Mapper(new MapperConfiguration(m =>
            {
                m.AddProfile<ProductShopProfile>();
            }));

            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> users = new HashSet<User>();
            foreach (ImportUserDto userDto in userDtos)
            {
                User user = mapper.Map<User>(userDto);

                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count}";
        }
    }
}