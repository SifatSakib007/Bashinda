using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class OTPViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string? OTP { get; set; }
    }
}
