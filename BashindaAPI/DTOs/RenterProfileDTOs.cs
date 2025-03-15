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
        
        // Address information
        public AddressDto Address { get; set; } = new AddressDto();
        
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateRenterProfileDto
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

        // Location information
        [Required]
        public int DivisionId { get; set; }

        [Required]
        public int DistrictId { get; set; }

        [Required]
        public int UpazilaId { get; set; }

        [Required]
        public AreaType AreaType { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        public int VillageId { get; set; }

        [Required]
        [StringLength(10)]
        public string PostCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HoldingNo { get; set; } = string.Empty;
    }

    public class UpdateRenterProfileDto
    {
        [Required]
        public bool IsAdult { get; set; }

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

        // Location information
        [Required]
        public int DivisionId { get; set; }

        [Required]
        public int DistrictId { get; set; }

        [Required]
        public int UpazilaId { get; set; }

        [Required]
        public AreaType AreaType { get; set; }

        [Required]
        public int WardId { get; set; }

        [Required]
        public int VillageId { get; set; }

        [Required]
        [StringLength(10)]
        public string PostCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string HoldingNo { get; set; } = string.Empty;
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
        
        // Basic location information
        public string DivisionName { get; set; } = string.Empty;
        public string DistrictName { get; set; } = string.Empty;
        public string UpazilaName { get; set; } = string.Empty;
    }

    public class ApproveRenterProfileDto
    {
        [Required]
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }

    public class UploadRenterImageDto
    {
        [Required]
        public IFormFile Image { get; set; } = null!;

        [Required]
        public string ImageType { get; set; } = string.Empty; // self, nationalid, birthregistration
    }
}
