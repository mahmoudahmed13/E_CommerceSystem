using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }
        //Login
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
            => ToActionResult(await _authenticationService.LoginAsync(loginDto));

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken ct)
            => ToActionResult(await _authenticationService.RegisterAsync(registerDto, ct));

        // Check Email Exists
        [HttpGet("emailexists")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery] string email, CancellationToken ct)
            => ToActionResult(await _authenticationService.CkeckEmailExitsAysnc(email, ct));

        // Get Current User
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
            => ToActionResult(await _authenticationService.GetCurrentUserAsync(GetEmailFromToken(), ct));

        // Get Current User Address
        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress(CancellationToken ct)
        => ToActionResult(await _authenticationService.GetUserAddressAsync(GetEmailFromToken(), ct));

        // Update Current User Address
        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto, CancellationToken ct)
           => ToActionResult(await _authenticationService.UpSertUserAddressAsync(GetEmailFromToken(), addressDto, ct));
    }
}
