using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {

            var context = new ProductShopContext();
            string inputJson = File.ReadAllText(@"../../../Datasets/categories-products.json");
            //string result = ImportCategoryProducts(context, inputJson);
            Console.WriteLine(GetCategoriesByProductsCount(context));

        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();
            List<ImportUserDto> usersDTO = JsonConvert.DeserializeObject<List<ImportUserDto>>(inputJson);
            ICollection<User> validUsers = new HashSet<User>();

            foreach (ImportUserDto userDto in usersDTO)
            {
                User user = mapper.Map<User>(userDto);
                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();
            return $"Successfully imported {validUsers.Count}";
        }
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();
            List<ImportProductDto> productDtos = JsonConvert.DeserializeObject<List<ImportProductDto>>(inputJson);
            ICollection<Product> ValidProducts = new HashSet<Product>();

            foreach (ImportProductDto productDto in productDtos)
            {
                Product user = mapper.Map<Product>(productDto);
                ValidProducts.Add(user);
            }

            context.Products.AddRange(ValidProducts);
            context.SaveChanges();
            return $"Successfully imported {ValidProducts.Count}";
        }
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = CreateMapper();
            List<ImportCategoriesDto> categoriesDto = JsonConvert.DeserializeObject<List<ImportCategoriesDto>>(inputJson);
            ICollection<Category> ValidCategories = new HashSet<Category>();

            foreach (ImportCategoriesDto categorieDto in categoriesDto)
            {
                if (categorieDto.Name != null)
                {
                    Category category = mapper.Map<Category>(categorieDto);
                    ValidCategories.Add(category);
                }

            }

            context.Categories.AddRange(ValidCategories);
            context.SaveChanges();
            return $"Successfully imported {ValidCategories.Count}";
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var mapper = CreateMapper();

            var categoryProductDtos = JsonConvert.DeserializeObject<ImportCategoryProductDto[]>(inputJson);
            var categoryProducts = new HashSet<CategoryProduct>();
            foreach (var item in categoryProductDtos)
            {
                CategoryProduct cp = mapper.Map<CategoryProduct>(item);
                categoryProducts.Add(cp);
            }

            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }
        private static IMapper CreateMapper()
        {
            return new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products.Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    name = x.Name,
                    price = x.Price,
                    seler = x.Seller.FirstName + " " + x.Seller.LastName
                }).AsNoTracking().ToArray();
            return JsonConvert.SerializeObject(products);
        }
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count != 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(ps => ps.Buyer != null).Select(ps => new
                    {
                        name = ps.Name,
                        price = ps.Price,
                        buyerFirstName = ps.Buyer.FirstName,
                        buyerLastName = ps.Buyer.LastName
                    })
                }).ToArray();
            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories.
            OrderByDescending(c => c.CategoriesProducts.Count)
            .Select(c => new
            {
                category = c.Name,
                productsCount = c.CategoriesProducts.Count,
                averagePrice = (c.CategoriesProducts.Any() ? c.CategoriesProducts.Average(cp => cp.Product.Price) : 0).ToString("f2"),
                totalRevenue = (c.CategoriesProducts.Any() ? c.CategoriesProducts.Sum(cp => cp.Product.Price) : 0).ToString("f2")
            }).AsNoTracking().ToArray();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold
                        .Count(p => p.Buyer != null),
                        products = u.ProductsSold
                        .Where(p => p.Buyer != null)
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })
                        .ToArray()
                    }

                })
                .OrderByDescending(u => u.soldProducts.count)
                .AsNoTracking()
                .ToArray();

            var usersWrapper = new
            {
                usersCount = users.Length,
                users = users
            };
            return JsonConvert.SerializeObject(usersWrapper, Formatting.Indented, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
        }
    }
}