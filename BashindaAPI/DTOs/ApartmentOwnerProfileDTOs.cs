using System.ComponentModel.DataAnnotations;
using BashindaAPI.Models;
using Microsoft.AspNetCore.Http;

namespace BashindaAPI.DTOs
{
    public class ApartmentOwnerProfileDto
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
        public Nationality Nationality { get; set; } 
        public BloodGroup BloodGroup { get; set; } 
        public Profession Profession { get; set; } 
        public Gender Gender { get; set; } 
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? SelfImagePath { get; set; }

        // Location information - string-based fields
        public string Division { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Upazila { get; set; } = string.Empty;
        public AreaType AreaType { get; set; } 
        public string Ward { get; set; } = string.Empty;
        public string Village { get; set; } = string.Empty;
        public string PostCode { get; set; } = string.Empty;
        public string HoldingNo { get; set; } = string.Empty;
        public string? UniqueId { get; set; }

        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateApartmentOwnerProfileDto
    {
        [Required]
        public bool IsAdult { get; set; }

        // If IsAdult = true, NationalId is required; otherwise BirthRegistrationNo is required
        public string? NationalId { get; set; }
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
        public string MobileNo { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // Location information - using strings instead of IDs
        [Required]
        [StringLength(100)]
        public string Division { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string District { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Upazila { get; set; } = string.Empty;

        [Required]
        public AreaType AreaType { get; set; } 

        [Required]
        [StringLength(100)]
        public string Ward { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Village { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string PostCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HoldingNo { get; set; } = string.Empty;
    }

    public class UpdateApartmentOwnerProfileDto
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string MobileNo { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // Location information - using strings instead of IDs
        [Required]
        [StringLength(100)]
        public string Division { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string District { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Upazila { get; set; } = string.Empty;

        [Required]
        public AreaType AreaType { get; set; }

        [Required]
        [StringLength(100)]
        public string Ward { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Village { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string PostCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HoldingNo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Profession { get; set; } = string.Empty;
    }

    public class ApartmentOwnerProfileListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public string? SelfImagePath { get; set; }
        public bool IsApproved { get; set; }
        public string DivisionName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string UpazilaName { get; set; } = string.Empty;
    }

    public class ApproveApartmentOwnerProfileDto
    {
        [Required]
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class UploadOwnerImageDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;

        [Required]
        public string ImageType { get; set; } = string.Empty; // self or nationalid
    }
}
