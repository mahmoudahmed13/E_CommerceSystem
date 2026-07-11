using E_Commerce.Infrastructure.Data;
using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class CatalogDataSeeder(StoreDbContext dbContext, ILogger<CatalogDataSeeder> logger)
        : IDataSeeder
    {
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                    await dbContext.Database.MigrateAsync(ct);

                var seedRoot =  Path.Combine(AppContext.BaseDirectory, "DataSeed");

                var path = Path.Combine(seedRoot, "products.json");

                await SeedIfEmptyAsync<ProductBrand, int>(seedRoot, "brands.json", ct);
                await SeedIfEmptyAsync<ProductType, int>(seedRoot, "types.json", ct);
                await SeedIfEmptyAsync<Product, int>(seedRoot, "products.json", ct);

                int result = await dbContext.SaveChangesAsync(ct);
                if (result > 0)
                    logger.LogInformation("Data seeding completed successfully.");
                else
                    logger.LogInformation("No new data was seeded.");
                
            }
			catch (Exception)
			{
			}
        }

        private async Task SeedIfEmptyAsync<T , TKey>(string rootPath, string fileName,
            CancellationToken ct = default)
            where T : BaseEntity<TKey>
        {
            if (await dbContext.Set<T>().AnyAsync())
            {
                logger.LogInformation("Table Has Already Data");
                return;
            }
            var filePath = Path.Combine(rootPath, fileName);
            if (!File.Exists(filePath)) 
            {
                logger.LogWarning($"File {filePath} does not exist");
                return; 
            }

            using var fileStream = File.OpenRead(filePath);
            var items = await JsonSerializer.DeserializeAsync<List<T>>(fileStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true}, ct);
            if (items?.Any() ?? false)
                dbContext.Set<T>().AddRange(items);
        }
    }
}
