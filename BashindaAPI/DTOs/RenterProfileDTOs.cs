using System.ComponentModel.DataAnnotations;
using BashindaAPI.Models;
using Microsoft.AspNetCore.Http;

namespace BashindaAPI.DTOs
{
    public class RenterProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public bool IsAdult { get; set; }
        public string? NationalId { get; set; }
        public string? NationalIdImagePath { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public string? BirthRegistrationImagePath { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Profession { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SelfImagePath { get; set; }
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateRenterProfileDto
    {
        [Required]
        public bool IsAdult { get; set; }

        // Adult fields (conditional)
        [StringLength(20)]
        public string? NationalId { get; set; }

        // Child fields (conditional)
        [StringLength(20)]
        public string? BirthRegistrationNo { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FatherName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string MotherName { get; set; } = string.Empty;

        [Required]
        public Nationality Nationality { get; set; }

        [Required]
        public BloodGroup BloodGroup { get; set; }

        [Required]
        public Profession Profession { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [StringLength(20)]
        [Phone]
        public string MobileNo { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Division { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string District { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Upazila { get; set; } = string.Empty;
    }

    public class UpdateRenterProfileDto
    {
        public bool IsAdult { get; set; }
        public string? NationalId { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        public Nationality Nationality { get; set; }
        public BloodGroup BloodGroup { get; set; }
        public Profession Profession { get; set; }
        public Gender Gender { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
    }

    public class RenterProfileListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? BirthRegistrationNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? SelfImagePath { get; set; }
        public bool IsApproved { get; set; }
    }

    public class ApproveRenterProfileDto
    {
        public bool IsApproved { get; set; }
    }

    public class UploadRenterImageDto
    {
        [Required]
        public IFormFile Image { get; set; }
        
        public string ImageType { get; set; } = "Self"; // "Self", "NationalId", or "BirthRegistration"
    }
}
