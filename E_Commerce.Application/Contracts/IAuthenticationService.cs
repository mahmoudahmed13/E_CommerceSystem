using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Identity;

namespace E_Commerce.Application.Contracts
{
    public interface IAuthenticationService
    {
        Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default);
    }
}
