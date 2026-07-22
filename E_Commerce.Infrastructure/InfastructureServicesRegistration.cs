using E_Commerce.Infrastructure.Data;
using E_Commerce.Infrastructure.DataSeeding;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using E_Commerce.Infrastructure.Repositories;
using StackExchange.Redis;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using E_Commerce.Application.Contracts;
using E_Commerce.Infrastructure.Identity.Services;

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

            services.AddDbContext<StoreIdentityDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
            });

            //services.AddScoped<IDataSeeder, CatalogDataSeeder>();
            services.AddKeyedScoped<IDataSeeder, CatalogDataSeeder>("catalog");
            services.AddKeyedScoped<IDataSeeder, IdentityDataSeeder>("Identity");
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddSingleton<IConnectionMultiplexer>(config =>
            {
                return ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")!);
            });

            services.AddScoped<IBasketRepository, BasketRepository>();

            services.AddSingleton<ICacheRepository, CacheRepository>();

            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<StoreIdentityDbContext>();

            services.AddScoped<IIdentityService, IdentityService>();

            return services;
        }
    }
}
