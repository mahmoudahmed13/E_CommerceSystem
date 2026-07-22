using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Application.DTOs.Identity
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;
        [Required]
        public string Password { get; set; } = default!;

    }
}
