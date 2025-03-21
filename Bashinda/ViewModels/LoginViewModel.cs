using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class LoginViewModel
    {

        [Required, EmailAddress]
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }

        [Required, DataType(DataType.Password)]
        public required string Password { get; set; }
        
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
