using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;

namespace E_Commerce.Application.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<Result<bool>> CkeckEmailExitsAysnc(string email, CancellationToken ct = default)
            => await _identityService.CheckEmailExistsAsync(email, ct);

        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken ct = default)
        {
            // Get User By Email
            var userResult = await _identityService.FindUserByEmailAsync(email);
            var user = userResult.data;

            var rolesResult = await _identityService.GetUserRoles(email,ct);
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, rolesResult.data);

            return new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = token
            };
        }

        public async Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken ct = default)
        {
            return await _identityService.GetAddressByEmailAsync(email, ct);
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            // Get User By Email
            var userResult = await _identityService.FindUserByEmailAsync(loginDto.Email);
            if (!userResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            // Check Password
            var passwordResult = await _identityService.CheckPasswordAsync(loginDto.Email, loginDto.Password);
            if (!passwordResult.IsSuccess)
                return Result<UserDto>.Fail(userResult.Errors);

            if(!passwordResult.data)
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid Email Or Passsword"));

            var user = userResult.data;
            var rolesResult = await _identityService.GetUserRoles(user.Email);
            var roles = rolesResult.data;
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);

            return new UserDto()
            {
                Email = userResult.data.Email,
                DisplayName = userResult.data.DisplayName,
                Token = token
            };

            // Return Result + User Dto
        }

        public async Task<Result<UserDto>> RegisterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var userResult =await _identityService.CreateUserAsync(registerDto, ct);

            if (!userResult.IsSuccess)
            {
                return Result<UserDto>.Fail(userResult.Errors);
            }
            var user = userResult.data;

            var rolesResult = await _identityService.GetUserRoles(user.Email);
            var roles = rolesResult.data;
            var token = _tokenService.CreateToken(user.Id, user.Email, user.UserName, roles);

            return Result<UserDto>.Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = token
            });

        }

        public async Task<Result<AddressDto>> UpSertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            return await _identityService.UpdateOrInsertUserAddressAsync(email, addressDto, ct);
        }
    }
}
 