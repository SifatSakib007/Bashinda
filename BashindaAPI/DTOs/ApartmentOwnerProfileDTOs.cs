using System.ComponentModel.DataAnnotations;
using BashindaAPI.Models;
using Microsoft.AspNetCore.Http;

namespace BashindaAPI.DTOs
{
    public class ApartmentOwnerProfileDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? NationalId { get; set; }
        public string? NationalIdImagePath { get; set; }
        public string? SelfImagePath { get; set; }
        public string MobileNo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public AddressDto Address { get; set; } = new AddressDto();
        public string Profession { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
        public UserDto? User { get; set; }
    }

    public class CreateApartmentOwnerProfileDto
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

        [Required]
        [StringLength(100)]
        public string Profession { get; set; } = string.Empty;
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
