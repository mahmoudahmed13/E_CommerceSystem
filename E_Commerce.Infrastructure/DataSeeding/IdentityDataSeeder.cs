using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Identity.Data;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace E_Commerce.Infrastructure.DataSeeding
{
    internal class IdentityDataSeeder : IDataSeeder
    {
        private readonly StoreIdentityDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<IdentityDataSeeder> _logger;

        public IdentityDataSeeder(StoreIdentityDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<IdentityDataSeeder> logger) 
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }
        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            try
            {
                //1) Check pending migrations
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync(ct);
                if (pendingMigrations.Any())
                    await _dbContext.Database.MigrateAsync(ct);
                //2) Create Roles
                if (!await _roleManager.Roles.AnyAsync(ct))
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                    await _roleManager.CreateAsync(new IdentityRole { Name = "SuperAdmin" });
                }
                //3) Create Users
                if (!await _userManager.Users.AnyAsync(ct))
                {
                    var admin = new ApplicationUser()
                    {
                        DisplayName = "Mahmoud Ahmed",
                        Email = "mahmoud@gmail.com",
                        UserName = "MahmoudAhmed",
                        PhoneNumber = "01245677543"
                    };

                    var createResult = await _userManager.CreateAsync(admin, "P@ssw0rd");

                    if (createResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(admin, "SuperAdmin");
                    }
                    else
                    {
                        var Errors = string.Join(';', createResult.Errors.Select(x => x.Description));
                        _logger.LogWarning($"Can Not Seed Default Admin {Errors}");
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Identity Data Seeding Failed");
                return;
            }
        }
    }
}
