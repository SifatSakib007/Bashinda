using System.ComponentModel.DataAnnotations;

namespace Bashinda.Models
{
    public enum UserRole
    {
        Admin,
        ApartmentOwner,
        ApartmentRenter
    }
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string PasswordHash { get; set; } // We'll store a hashed password

        public bool IsVerified { get; set; }

        // Default role after verification
        [Required]
        public UserRole Role { get; set; } 

        
    }
}
