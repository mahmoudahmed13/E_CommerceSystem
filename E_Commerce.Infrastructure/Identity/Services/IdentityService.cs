using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<bool>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Is Not Found"));
            else
                return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<IdentityUserResult>.Fail(Error.NotFound("User Not Found", $"User With Email {email} Is Not Found"));
            else
                return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }
    }
}
