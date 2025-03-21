using System.ComponentModel.DataAnnotations;

namespace Bashinda.Models
{
    public enum UserRole
    {
        SuperAdmin,
        Admin,
        ApartmentOwner,
        ApartmentRenter
    }
    public class User
    {
        public int Id { get; set; }

        [Required]
        public required string UserName { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, Phone]
        public required string PhoneNumber { get; set; }

        [Required]
        public required string PasswordHash { get; set; } // We'll store a hashed password

        public bool IsVerified { get; set; }

        // Default role after verification
        [Required]
        public UserRole Role { get; set; } 

        
    }
}
