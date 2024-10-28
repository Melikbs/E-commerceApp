using Core.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
        {


            try
            {
                if (!context.ProductBrands.Any())
                {

                    var brandsData = File.ReadAllText("C://Users//melik//source//repos//E-commerceApp//Infrastructure//Data//SeedData//brands.json");
                    var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                    await context.ProductBrands.AddRangeAsync(brands); // Use AddRange for multiple inserts
                    await context.SaveChangesAsync();

                }


                if (!context.ProductTypes.Any())
                {

                    var typesData = File.ReadAllText("C://Users//melik//source//repos//E-commerceApp//Infrastructure//Data//SeedData//types.json");
                    var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);
                    await context.ProductTypes.AddRangeAsync(types); // Use AddRange for multiple inserts
                    await context.SaveChangesAsync();

                }

                if (!context.Products.Any())
                {

                    var productsData = File.ReadAllText("C://Users//melik//source//repos//E-commerceApp//Infrastructure//Data//SeedData//products.json");
                    var products = JsonSerializer.Deserialize<List<Product>>(productsData);
                    await context.Products.AddRangeAsync(products); // Use AddRange for multiple inserts
                    await context.SaveChangesAsync();

                }

            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

    }
}