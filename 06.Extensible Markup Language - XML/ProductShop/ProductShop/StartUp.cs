using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.DTOs.Export;
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
            //string inputXml = File.ReadAllText("../../../Datasets/categories-products.xml");

            string result = GetUsersWithProducts(context);
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


        //Query 2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportProductsDto[] importProductDtos = xmlHelper.Deserialize<ImportProductsDto[]>(inputXml, "Products");

            ICollection<Product> products = new HashSet<Product>();
            foreach (var item in importProductDtos)
            {
                Product product = mapper.Map<Product>(item);
                products.Add(product);
            }
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count}";
        }

        //Query 3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportCategoriesDto[] importCategories = xmlHelper.Deserialize<ImportCategoriesDto[]>(inputXml, "Categories");
            ICollection<Category> categories = new HashSet<Category>();
            foreach (var item in importCategories)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }
                Category category = mapper.Map<Category>(item);
                categories.Add(category);
            }
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count}";
        }

        //Query 4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportCategoryProductsDto[] importCategoryProductsDtos = xmlHelper.Deserialize<ImportCategoryProductsDto[]>(inputXml, "CategoryProducts");
            ICollection<CategoryProduct> categories = new HashSet<CategoryProduct>();
            foreach (var item in importCategoryProductsDtos)
            {
                if (!context.Products.Any(c=>c.Id == item.ProductId) || 
                    !context.Categories.Any(c=>c.Id == item.CategoryId)) //|| !context.Cars.Any(c => c.Id == saleDto.CarId.Value))
                {
                    continue;
                }
                CategoryProduct category = mapper.Map<CategoryProduct>(item);
                categories.Add(category);
            }
            context.CategoryProducts.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count}";
        }


        //Query 5. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportProductsInRangeDto[] productsInRangeDtos = context.Products
                .Where(p=>p.Price>=500 && p.Price<=1000)   //price range between 500 and 1000 (inclusive
                .OrderBy(p=>p.Price)
                .Take(10)
                .ProjectTo<ExportProductsInRangeDto>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize(productsInRangeDtos, "Products");
        }


        //Query 6. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportUserSoldProductsDto[] exportUsers = context.Users
                 .Where(u => u.ProductsSold.Any())
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .ProjectTo<ExportUserSoldProductsDto>(mapper.ConfigurationProvider)
                .ToArray();

            return xmlHelper.Serialize(exportUsers, "Users");
        }


        //Query 7. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCategoryDto[] categoryDtos = context.Categories
                .ProjectTo<ExportCategoryDto>(mapper.ConfigurationProvider)
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            return xmlHelper.Serialize(categoryDtos, "Categories");
        }


        //Query 8. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            XmlHelper xmlHelper = new XmlHelper();

            var usersInfo = context
                .Users
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new UserInfo()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsCount()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(p => new SoldProduct()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            ExportUserCountDto exportUserCountDto = new ExportUserCountDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = usersInfo
            };

            return xmlHelper.Serialize(exportUserCountDto, "Users");
        }



        private static IMapper InitializeAutoMapper() =>
            new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
    }
}