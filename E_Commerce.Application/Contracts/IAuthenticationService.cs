using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Authentications;

namespace E_Commerce.Application.Contracts
{
    public interface IAuthenticationService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
        Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default);
        Task<Result<bool>> CkeckEmailExitsAysnc(string email, CancellationToken ct = default);
        Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default);
        Task<Result<AddressDto>> UpSertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default);
    }
}
