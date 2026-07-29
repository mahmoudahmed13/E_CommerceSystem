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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using E_Commerce.Infrastructure.Payments;

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
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPaymentGateway, StripePaymentGateway>();

            var jwtSetting = configuration.GetSection("JWT").Get<JwtSetting>()
                ?? throw new InvalidOperationException("Jwt Settings is missing");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opt =>
            {
                opt.SaveToken = true;
                opt.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSetting.Issuer, // لازم يطابق الـ issuer اللي بعتناه
                    ValidateAudience = true,
                    ValidAudience = jwtSetting.Audience,  // لازم يطابق الـ audience
                    ValidateLifetime = true,   // يتأكد إن exp لسه ساري
                    RequireExpirationTime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                }; 
            });

            return services;
        }
    }
}
