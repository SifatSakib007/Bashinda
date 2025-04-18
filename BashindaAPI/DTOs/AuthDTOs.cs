using BashindaAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BashindaAPI.DTOs
{
    public class LoginDto
    {
        [Phone]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto : IValidatableObject
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Role == UserRole.ApartmentOwner || Role == UserRole.ApartmentRenter)
            {
                if (!Regex.IsMatch(Password, @"^\d+$"))
                {
                    yield return new ValidationResult(
                        "Please enter only numbers.",
                        new[] { nameof(Password) });
                }
            }
        }
    }
    

    public class VerifyOtpDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string OtpCode { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } = string.Empty;
        public UserDto? User { get; set; }
        public string[] Errors { get; set; } = Array.Empty<string>();
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
    }
} 