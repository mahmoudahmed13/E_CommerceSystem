using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.DataSeeding;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using E_Commerce.Infrastructure.Repositories;
using StackExchange.Redis;

namespace E_Commerce.Infrastructure
{
    public static class InfastructureServicesRegistration
    {
        public static IServiceCollection AddInfastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            //services.AddScoped<IDataSeeder, CatalogDataSeeder>();
            services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("catalog");
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IConnectionMultiplexer>(config =>
            {
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!);
            });

            //services.AddSingleton<IConnectionMultiplexer>(config =>
            //{
            //    var configOptions = new ConfigurationOptions
            //    {
            //        EndPoints = { "127.0.0.1:6379" },
            //        AbortOnConnectFail = false // <-- الأهم
            //    };
            //    return ConnectionMultiplexer.Connect(configOptions);
            //});
            services.AddScoped<IBasketRepository, BasketRepository>();
            return services;
        }
    }
}
