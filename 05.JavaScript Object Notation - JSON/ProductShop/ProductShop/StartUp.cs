using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
            //string inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json");

            string result = GetCategoriesByProductsCount(context);
            Console.WriteLine(result);
        }

        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var mapper = new Mapper(new MapperConfiguration(m =>
            {
                m.AddProfile<ProductShopProfile>();
            }));

            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            var users = new HashSet<User>();
            foreach (ImportUserDto userDto in userDtos)
            {
                User user = mapper.Map<User>(userDto);

                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count}";
        }


        //Query 2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var mapper = new Mapper(new MapperConfiguration(m =>
            {
                m.AddProfile<ProductShopProfile>();
            }));

            ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            //Product[] products = mapper.Map<Product[]>(productDtos);
            ICollection<Product> products = new HashSet<Product>();

            foreach (var p in productDtos)
            {
                Product product = mapper.Map<Product>(p);

                products.Add(product);
            }
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count}";
        }

        //Query 3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var mapper = new Mapper(new MapperConfiguration(m =>
            {
                m.AddProfile<ProductShopProfile>();
            }));

            var categoriesDtos = JsonConvert.DeserializeObject<ImportCategoryDto[]>(inputJson);

            //Category[] categories = mapper.Map<Category[]>(categoriesDtos);
            var categories = new HashSet<Category>();
            foreach (var c in categoriesDtos)
            {
                if (!String.IsNullOrEmpty(c.Name))
                {
                    Category category = mapper.Map<Category>(c);
                    categories.Add(category);
                }
                
            }
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count}";
        }


        //Query 4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var mapper = new Mapper(new MapperConfiguration(m =>
            {
                m.AddProfile<ProductShopProfile>();
            }));

            ImportCategoryProductsDto[] categoryProductDtos = JsonConvert.DeserializeObject<ImportCategoryProductsDto[]>(inputJson);

            //CategoryProduct[] categoryProducts = mapper.Map<CategoryProduct[]>(categoryProductDtos);
            ICollection<CategoryProduct> categoryProducts = new HashSet<CategoryProduct>();
            foreach (ImportCategoryProductsDto cp in categoryProductDtos)
            {
                //if (!context.Categories.Any(c => c.Id == cp.CategoryId) ||
                //    !context.Products.Any(p=>p.Id == cp.ProductId))
                //{
                //    continue;
                //}
                CategoryProduct category = mapper.Map<CategoryProduct>(cp);
                     categoryProducts.Add(category);              

            }
            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Count}";
        }


        //Query 5. Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name=p.Name,
                    price=p.Price,
                    seller=p.Seller.FirstName + " " + p.Seller.LastName
                });

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }


        //Query 6. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context.Users
                .Where(p => p.ProductsSold.Any(u=>u.Buyer != null))
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Select(p => new
            {
                firstName = p.FirstName,
                lastName = p.LastName,
                soldProducts = p.ProductsSold.Where(p=>p.Buyer != null)
                .Select(b => new
                {
                    name = b.Name,
                    price = b.Price,
                    buyerFirstName = b.Buyer.FirstName,
                    buyerLastName = b.Buyer.LastName,
                }).ToArray()
            }).AsNoTracking().ToArray();

            return JsonConvert.SerializeObject(soldProducts, Formatting.Indented);
        }


        //Query 7. Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var result = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = (//c.CategoriesProducts.Any() ?
                    c.CategoriesProducts.Average(cp => cp.Product.Price)).ToString("f2"),
                    totalRevenue = (//c.CategoriesProducts.Any() ?
                    c.CategoriesProducts.Sum(cp => cp.Product.Price)).ToString("f2")
                }).AsNoTracking().ToArray();

                 return JsonConvert.SerializeObject(result, Formatting.Indented);
        }
    }
}