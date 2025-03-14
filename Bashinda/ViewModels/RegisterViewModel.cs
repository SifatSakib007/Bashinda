using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public required string UserName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, Phone]
        public required string PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please select a role.")]
        public required string Role { get; set; }
    }
}
