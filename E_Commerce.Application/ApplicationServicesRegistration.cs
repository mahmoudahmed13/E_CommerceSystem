using E_Commerce.Application.Contracts;
using E_Commerce.Application.Profiles;
using E_Commerce.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace E_Commerce.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services here
            services.AddAutoMapper(c => { }, typeof(ApplicationServicesRegistration).Assembly);
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IOrderService, OrderService>();
            // Register AutoMapper profiles from the current assembly
            return services;
        }
    }
}
