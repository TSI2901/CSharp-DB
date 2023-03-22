using CarDealer.Data;
using CarDealer.DTOs.Import;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CarDealer.Models;
using Castle.Core.Resource;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            string stringJson = File.ReadAllText("../../../Datasets/sales.json");

            Console.WriteLine(GetCarsFromMakeToyota(context));
        }
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var mapper = CreateMapper();

            var supplierDto = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);
            var suppliers = mapper.Map<Supplier[]>(supplierDto);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var mapper = CreateMapper();

            var partDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            var parts = new HashSet<Part>();
            foreach (var partDto in partDtos)
            {
                if (!context.Suppliers.Any(s => s.Id == partDto.SupplierId))
                {
                    continue;
                }
                var part = mapper.Map<Part>(partDto);
                parts.Add(part);
            }
            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}.";
        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var mapper = CreateMapper();
            ImportCarDto[] importCarDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);
            ICollection<Car> validCars = new HashSet<Car>();
            ICollection<PartCar> validParts = new HashSet<PartCar>();

            foreach (var importCarDto in importCarDtos)
            {
                Car car = mapper.Map<Car>(importCarDto);
                validCars.Add(car);

                foreach (var partId in importCarDto.PartsId.Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        Car = car,
                        PartId = partId,
                    };
                    validParts.Add(partCar);
                }
            }
            context.Cars.AddRange(validCars);
            context.PartsCars.AddRange(validParts);
            context.SaveChanges();
            return $"Successfully imported {validCars.Count}.";
        }
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var mapper = CreateMapper();

            var customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);
            
            var customers = mapper.Map<Customer[]>(customerDtos);

            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}.";
        }
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var mapper = CreateMapper();

            var salesDto = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

            var sales = mapper.Map<Sale[]>(salesDto);
            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Length}.";
        }
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers.OrderBy(x => x.BirthDate).ThenBy(x => x.IsYoungDriver).Select(x => new
            {
                Name = x.Name,
                BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                IsYoungDriver = x.IsYoungDriver,
            }).ToList();
            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars.Where(c => c.Make == "Toyota").OrderBy(c => c.Model).ThenByDescending(c => c.TravelledDistance).Select(c => new
            {
                Id = c.Id,
                Make = c.Make,
                Model = c.Model,
                TravelledDistance = c.TravelledDistance,
            }).AsNoTracking().ToList();
            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }
        private static IMapper CreateMapper() => new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CarDealerProfile>();
        }));
    }
}