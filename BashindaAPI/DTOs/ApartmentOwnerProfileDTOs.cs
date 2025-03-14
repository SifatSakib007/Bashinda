using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BashindaAPI.DTOs
{
    public class ApartmentOwnerProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string NationalId { get; set; } = string.Empty;
        public string? NationalIdImagePath { get; set; }
        public string? SelfImagePath { get; set; }
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Profession { get; set; }
        public bool IsApproved { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateApartmentOwnerProfileDto
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "National ID")]
        public string NationalId { get; set; } = string.Empty;

        [Display(Name = "Contact Number")]
        [Phone]
        [StringLength(15)]
        public string? MobileNo { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Display(Name = "Address")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Profession")]
        [StringLength(100)]
        public string? Profession { get; set; }
    }

    public class UpdateApartmentOwnerProfileDto
    {
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        [Display(Name = "National ID")]
        public string NationalId { get; set; } = string.Empty;

        [Display(Name = "Contact Number")]
        [Phone]
        [StringLength(15)]
        public string? MobileNo { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Display(Name = "Address")]
        [StringLength(200)]
        public string? Address { get; set; }

        [Display(Name = "Profession")]
        [StringLength(100)]
        public string? Profession { get; set; }
    }

    public class ApartmentOwnerProfileListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? MobileNo { get; set; }
        public string? Email { get; set; }
        public string? NationalId { get; set; }
        public string? SelfImagePath { get; set; }
        public bool IsApproved { get; set; }
    }

    public class ApproveApartmentOwnerProfileDto
    {
        public bool IsApproved { get; set; }
    }

    public class UploadOwnerImageDto
    {
        [Required]
        public IFormFile Image { get; set; }
        
        public string ImageType { get; set; } = "Self"; // "Self" or "NationalId"
    }
}
