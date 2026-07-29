using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var user = new ApplicationUser()
            {
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                UserName = registerDto.UserName,
                DisplayName = registerDto.DisplayName,
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(errors);
            }

            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user == null)
                return Error.NotFound("User Not Found",$"User with email : {email} is not found");

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user is not null; // true/false
        }

        public async Task<Result<AddressDto>> GetAddressByEmailAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);

            if (user?.Address == null) return Result<AddressDto>.Fail(Error.NotFound("Address Cot Found", $"Address of user with email {email} is not found"));
            
            var address = user.Address;
            return new AddressDto()
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                City = address.City,
                Street = address.Street,
                Country = address.Country,
            };
        }

        public async Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto address, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email, ct);
            if(user?.Address == null)
            {
                // Insert
                user.Address = new Address()
                {
                    FirstName = address.FirstName,
                    LastName = address.LastName,
                    City = address.City,
                    Street = address.Street,
                    Country = address.Country,
                };
            }
            else
            {
                // Update
                user.Address.FirstName = address.FirstName;
                user.Address.LastName = address.LastName;
                user.Address.Street = address.Street;
                user.Address.Country = address.Country;
                user.Address.City = address.City;
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return address;
            else
                return Error.Failure("Failure", string.Join(';', result.Errors.Select(e => e.Description)));
        }
    }
}
