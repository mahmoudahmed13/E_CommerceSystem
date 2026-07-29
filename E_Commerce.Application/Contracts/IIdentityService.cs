using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Authentications;

namespace E_Commerce.Application.Contracts
{
    public interface IIdentityService
    {
        Task<Result<IdentityUserResult>> FindUserByEmailAsync(string email, CancellationToken ct = default);
        Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken ct = default);
        Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken ct = default);
        Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken ct = default);     
        Task<Result<bool>> CheckEmailExistsAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> GetAddressByEmailAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> UpdateOrInsertUserAddressAsync(string email, AddressDto address, CancellationToken ct = default);
    }
}
