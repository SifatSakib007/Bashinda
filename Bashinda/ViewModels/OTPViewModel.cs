using System.ComponentModel.DataAnnotations;

namespace Bashinda.ViewModels
{
    public class OTPViewModel
    {

        [EmailAddress]
        public string? Email { get; set; }


        [Phone]
        public string? PhoneNumber { get; set; }


        [StringLength(6, MinimumLength = 6)]
        public string? OTP { get; set; }
    }
}
